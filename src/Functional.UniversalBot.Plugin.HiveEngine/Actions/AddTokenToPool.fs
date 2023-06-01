module AddTokenToPool 

open Action
open Hive
open HiveEngineTypes
open Some
open Decimal
open Types
open PipelineResult
open Hive
open Functional.ETL.Pipeline
open Functional.ETL.Pipeline.PipelineProcessData

[<Literal>]
let ModuleName = "AddToPool"

let private splitPair (tokenPair: string) = 
    let parts = tokenPair.Split(":")
    (parts[0], parts[1])

let private scheduleTokenToPoolTransfer username tokenPair baseQuantity quoteQuantity =
    bindCustomJson 
        "marketpools" 
        "addLiquidity" 
        {| 
            tokenPair = tokenPair
            baseQuantity = String.asString baseQuantity
            quoteQuantity = String.asString quoteQuantity
            maxSlippage = "1"
            maxDeviation = "0"
        |}
    |> buildCustomJson username "ssc-mainnet-hive"
    |> scheduleActiveOperation ModuleName tokenPair

let action hive hiveEngineUrl tokenPair leftAmountCalculator rightAmountCalculator username (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    let marketPosition = HiveEngine.getAvailableMarketPools hiveEngineUrl tokenPair
    let (leftToken, rightToken) = splitPair tokenPair
        
    let leftTokenPrecision = TokenInfo.getTokenPrecision entity leftToken
    let rightTokenPrecision = TokenInfo.getTokenPrecision entity rightToken

    let leftTokenBaseAmount =
        readPropertyAsDecimal entity leftToken 
        |> defaultWhenNone 0M
        |> leftAmountCalculator
        |> roundToPrecision leftTokenPrecision
    let leftTokenQuoteAmount = (leftTokenBaseAmount * marketPosition.basePrice) |> roundToPrecision rightTokenPrecision

    let rightTokenBaseAmount = 
        readPropertyAsDecimal entity rightToken
        |> defaultWhenNone 0M
        |> rightAmountCalculator
        |> roundToPrecision rightTokenPrecision
    let rightTokenQuoteAmount = (rightTokenBaseAmount * marketPosition.quotePrice) |> roundToPrecision leftTokenPrecision


    match (leftTokenBaseAmount, leftTokenQuoteAmount, rightTokenBaseAmount, rightTokenQuoteAmount) with 
    | (leftBase, leftQuote, _, _) when leftBase > 0M && leftQuote > 0M && leftBase <= leftTokenBaseAmount && leftQuote <= rightTokenBaseAmount ->
        scheduleTokenToPoolTransfer username tokenPair leftBase leftQuote |> withResult entity
    | (_, _, rightBase, rightQuote) when rightBase > 0M && rightQuote > 0M && rightBase <= rightTokenBaseAmount && rightQuote <= leftTokenBaseAmount ->
        scheduleTokenToPoolTransfer username tokenPair rightQuote rightBase |> withResult entity
    | _ -> 
        TokenBalanceTooLow (ModuleName, username, tokenPair) |> withResult entity 

let bind (urls: Urls) (parameters: Map<string, string>) = 
    let tokenPair = parameters.["tokenPair"]
    let leftAmount = parameters.["leftAmount"] |> AmountCalator.bind
    let rightAmount = parameters.["rightAmount"] |> AmountCalator.bind
    Action.bindAction ModuleName (action urls.hiveNodeUrl urls.hiveEngineNodeUrl tokenPair leftAmount rightAmount)
