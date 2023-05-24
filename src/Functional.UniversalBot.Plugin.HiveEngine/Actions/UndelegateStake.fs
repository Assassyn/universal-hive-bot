module UndelegateStake 

open Action
open Some
open PipelineResult
open Functional.ETL.Pipeline
open Functional.ETL.Pipeline.PipelineProcessData

[<Literal>]
let ModuleName = "UndelegateStake"

let action tokenSymbol undelegateFrom amountCalcualtor (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    let userDetails: (string * string * string) option = readPropertyAsType entity "userdata" 

    match userDetails with 
    | Some (username, _, _) when username <> "" -> 
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
    | _ -> 
        NoUserDetails ModuleName |> withResult entity
let bind urls (parameters: Map<string, string>) = 
    let token = parameters.["token"]
    let undelegateFrom = parameters.["undelegateFrom"]
    let amount = parameters.["amount"] |> AmountCalator.bind
    action token undelegateFrom amount
