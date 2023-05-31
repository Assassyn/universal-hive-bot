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

let action hive hiveEngineUrl tokenPair tokenSymbol maxSlippage amountToSwapCalcualtor (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    let userDetails: (string * string * string) option = readPropertyAsType entity "userdata" 

    match userDetails with 
    | Some (username, _, _) when username <> "" -> 
        let amountToSwap = 
            tokenSymbol
            |> readPropertyAsDecimal entity
            |> defaultWhenNone 0M
            |> amountToSwapCalcualtor

        if amountToSwap > 0M
        then 
            bindCustomJson "marketpools" "swapTokens" {| tokenPair = tokenPair; tokenSymbol = tokenSymbol; tokenAmount = String.asString amountToSwap; tradeType = "exactInput"; maxSlippage = maxSlippage |}
            |> buildCustomJson username "ssc-mainnet-hive"
            |> scheduleActiveOperation ModuleName tokenSymbol
            |> withResult entity
        else 
            TokenBalanceTooLow (ModuleName, username, tokenSymbol) |> PipelineProcessData.withResult entity
    | _ -> 
        NoUserDetails ModuleName |> PipelineProcessData.withResult entity

let bind (urls: Urls) (parameters: Map<string, string>) = 
    let tokenPair = parameters.["tokenPair"]
    let tokenSymbol = parameters.["token"]
    let maxSlippage = parameters.["maxSlippage"]
    let amountToSwap = parameters.["amountToSwap"] |> AmountCalator.bind
    action urls.hiveNodeUrl urls.hiveEngineNodeUrl tokenPair tokenSymbol maxSlippage amountToSwap
