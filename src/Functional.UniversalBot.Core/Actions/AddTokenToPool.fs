module AddTokenToPool 

open Action
open HiveEngineTypes
open Some
open Decimal
open Types
open PipelineResult
open Functional.ETL.Pipeline
open Functional.ETL.Pipeline.PipelineProcessData

[<Literal>]
let ModuleName = "AddToPool"

let private splitPair (tokenPair: string) = 
    let parts = tokenPair.Split(":")
    (parts[0], parts[1])

let private scheduleTokenToPoolTransfer logger username tokenPair baseQuantity quoteQuantity =
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
    |> scheduleActiveOperation (logger username) ModuleName tokenPair

let action logger hive hiveEngineUrl tokenPair leftAmountCalculator rightAmountCalculator (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    let userDetails: (string * string * string) option = PipelineProcessData.readPropertyAsType entity "userdata" 

    match userDetails with 
    | Some (username, _, _) when username <> "" -> 
        let marketPosition = HiveEngine.getAvailableMarketPools hiveEngineUrl tokenPair
        let (leftToken, rightToken) = splitPair tokenPair
        
        let tokensDetails = entity.properties.["tokenDetails"] :?> TokenInfo seq
        let leftTokenPrecision = TokenInfo.getTokenPrecision tokensDetails leftToken
        let rightTokenPrecision = TokenInfo.getTokenPrecision tokensDetails rightToken

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
            scheduleTokenToPoolTransfer logger username tokenPair leftBase leftQuote |> withResult entity
        | (_, _, rightBase, rightQuote) when rightBase > 0M && rightQuote > 0M && rightBase <= rightTokenBaseAmount && rightQuote <= leftTokenBaseAmount ->
            scheduleTokenToPoolTransfer logger username tokenPair rightBase rightQuote |> withResult entity
        | _ -> 
            TokenBalanceTooLow (ModuleName, tokenPair) |> withResult entity 
    | _ -> 
        NoUserDetails ModuleName |> PipelineProcessData.withResult entity

let bind logger (urls: Urls) (parameters: Map<string, string>) = 
    let tokenPair = parameters.["tokenPair"]
    let leftAmount = parameters.["leftAmount"] |> AmountCalator.bind
    let rightAmount = parameters.["rightAmount"] |> AmountCalator.bind
    action (logger ModuleName) urls.hiveNodeUrl urls.hiveEngineNodeUrl tokenPair leftAmount rightAmount
