module DelegateStake 

open Action
open Hive
open Decimal
open HiveEngineTypes
open Some
open PipelineResult
open Pipeline
open PipelineProcessData

[<Literal>]
let ModuleName = "DelegateStake"

let action tokenSymbol delegateTo amountCalcualtor username (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    let precision = TokenInfo.getTokenPrecision entity tokenSymbol
    let tokenBalance =
        sprintf "%s_stake" tokenSymbol
        |> readPropertyAsDecimal entity
        |> defaultWhenNone 0M
        |> amountCalcualtor
        |> roundToPrecision precision

    if tokenBalance > 0M
    then 
        bindCustomJson "tokens" "delegate" {|``to`` = delegateTo;symbol = tokenSymbol;quantity = String.asString tokenBalance|}
        |> buildActiveKeyedCustomJson username "ssc-mainnet-hive" 
        |> scheduleActiveOperation ModuleName tokenSymbol 
        |> withResult entity 
    else 
        TokenBalanceTooLow (ModuleName, username, tokenSymbol) |> withResult entity

let bind urls (parameters: Map<string, string>) = 
    let token = parameters.["token"]
    let delegateTo = parameters.["delegateTo"]
    let amount = parameters.["amount"] |> AmountCalator.bind

    Action.bindAction ModuleName (action token delegateTo amount)
