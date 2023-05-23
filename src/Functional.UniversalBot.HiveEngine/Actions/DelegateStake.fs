module DelegateStake 

open Action
open Decimal
open HiveEngineTypes
open Some
open PipelineResult
open Functional.ETL.Pipeline
open Functional.ETL.Pipeline.PipelineProcessData

[<Literal>]
let ModuleName = "DelegateStake"

let action logger tokenSymbol delegateTo amountCalcualtor (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    let userDetails: (string * string * string) option = readPropertyAsType entity "userdata" 

    match userDetails with 
    | Some (username, activeKey, _) -> 
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
            |> buildCustomJson username "ssc-mainnet-hive" 
            |> scheduleActiveOperation (logger username) ModuleName tokenSymbol 
            |> withResult entity 
        else 
            TokenBalanceTooLow (ModuleName, tokenSymbol) |> withResult entity
    | _ -> 
        NoUserDetails ModuleName |> withResult entity

let bind logger urls (parameters: Map<string, string>) = 
    let token = parameters.["token"]
    let delegateTo = parameters.["delegateTo"]
    let amount = parameters.["amount"] |> AmountCalator.bind
    action (logger ModuleName) token delegateTo amount
