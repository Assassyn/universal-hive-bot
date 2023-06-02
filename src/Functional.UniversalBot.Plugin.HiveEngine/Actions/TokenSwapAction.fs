module TokenSwapAction

open Action
open Hive
open Some
open Types
open PipelineResult
open Functional.ETL.Pipeline
open Functional.ETL.Pipeline.PipelineProcessData

[<Literal>]
let ModuleName = "SwapToken"

let action hive hiveEngineUrl tokenPair tokenSymbol maxSlippage amountToSwapCalcualtor username (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    let amountToSwap = 
        tokenSymbol
        |> readPropertyAsDecimal entity
        |> defaultWhenNone 0M
        |> amountToSwapCalcualtor

    if amountToSwap > 0M
    then 
        bindCustomJson "marketpools" "swapTokens" {| tokenPair = tokenPair; tokenSymbol = tokenSymbol; tokenAmount = String.asString amountToSwap; tradeType = "exactInput"; maxSlippage = maxSlippage |}
        |> buildActiveKeyedCustomJson username "ssc-mainnet-hive"
        |> scheduleActiveOperation ModuleName tokenSymbol
        |> withResult entity
    else 
        TokenBalanceTooLow (ModuleName, username, tokenSymbol) |> PipelineProcessData.withResult entity

let bind (urls: Urls) (parameters: Map<string, string>) = 
    let tokenPair = parameters.["tokenPair"]
    let tokenSymbol = parameters.["token"]
    let maxSlippage = parameters.["maxSlippage"]
    let amountToSwap = parameters.["amountToSwap"] |> AmountCalator.bind
    Action.bindAction ModuleName (action urls.hiveNodeUrl urls.hiveEngineNodeUrl tokenPair tokenSymbol maxSlippage amountToSwap)
