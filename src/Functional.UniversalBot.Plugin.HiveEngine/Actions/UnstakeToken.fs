module UnstakeToken

open Action
open Hive
open Some
open PipelineResult
open Pipeline
open PipelineProcessData

[<Literal>]
let ModuleName = "Unstake"

let action tokenSymbol amountCalcualtor username (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    let tokenBalance = 
        sprintf "%s_stake" tokenSymbol
        |> readPropertyAsDecimal entity
        |> defaultWhenNone 0M
        |> amountCalcualtor
    if tokenBalance > 0M
    then 
        bindCustomJson "tokens" "unstake" {|symbol = tokenSymbol;quantity = String.asString tokenBalance|}
        |> buildActiveKeyedCustomJson username "ssc-mainnet-hive" 
        |> scheduleActiveOperation ModuleName tokenSymbol 
        |> withResult entity 
    else 
        TokenBalanceTooLow (ModuleName, username, tokenSymbol) |> PipelineProcessData.withResult entity

let bind urls (parameters: Map<string, string>) = 
    let token = parameters.["token"]
    let amount = parameters.["amount"] |> AmountCalator.bind
    Action.bindAction ModuleName (action token amount)
