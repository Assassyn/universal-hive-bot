module UndelegateStake 

open Action
open Hive
open Some
open PipelineResult
open Functional.ETL.Pipeline
open Functional.ETL.Pipeline.PipelineProcessData

[<Literal>]
let ModuleName = "UndelegateStake"

let action tokenSymbol undelegateFrom amountCalcualtor username (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    let tokenBalance = 
        sprintf "%s_delegatedstake" tokenSymbol
        |> readPropertyAsDecimal entity
        |> defaultWhenNone 0M
        |> amountCalcualtor

    if tokenBalance > 0M
    then
        bindCustomJson "tokens" "undelegate" {|from = undelegateFrom;symbol = tokenSymbol;quantity = String.asStringWithPrecision tokenBalance|}
        |> buildCustomJson username "ssc-mainnet-hive" 
        |> scheduleActiveOperation ModuleName tokenSymbol 
        |> withResult entity 
    else 
        TokenBalanceTooLow (ModuleName, username, tokenSymbol) |> withResult entity

let bind urls (parameters: Map<string, string>) = 
    let token = parameters.["token"]
    let undelegateFrom = parameters.["undelegateFrom"]
    let amount = parameters.["amount"] |> AmountCalator.bind
    Action.bindAction ModuleName (action token undelegateFrom amount)
