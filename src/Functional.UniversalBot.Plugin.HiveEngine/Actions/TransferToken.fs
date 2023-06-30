module TransferToken 

open Action
open Hive
open PipelineResult
open Some
open Pipeline
open PipelineProcessData
open HiveEngineTypes
open Decimal

[<Literal>]
let ModuleName = "Transfer"

let action tokenSymbol transferTo amountCalcualtor memo username (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    let precision = TokenInfo.getTokenPrecision entity tokenSymbol
    let tokenBalance = 
        readPropertyAsDecimal entity tokenSymbol 
        |> defaultWhenNone 0M
        |> amountCalcualtor
        |> roundToPrecision precision

    if tokenBalance > 0M
    then 
        bindCustomJson "tokens" "transfer" {| ``to`` = transferTo;symbol = tokenSymbol;quantity = String.asString tokenBalance; memo = memo|}
        |> buildActiveKeyedCustomJson username "ssc-mainnet-hive" 
        |> scheduleActiveOperation ModuleName tokenSymbol 
        |> withResult entity 
    else 
        TokenBalanceTooLow (ModuleName, username, tokenSymbol) |> PipelineProcessData.withResult entity

let bind  urls (parameters: Map<string, string>) = 
    let tokenSymbol = parameters.["token"]
    let transferTo = parameters.["transferTo"]
    let memo = Map.getValueWithDefault parameters "memo" ""
    let amount = parameters.["amount"] |> AmountCalator.bind
    Action.bindAction ModuleName (action tokenSymbol transferTo amount memo)
