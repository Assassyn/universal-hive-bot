module UndelegateStake 

open Action
open FunctionalString
open Some
open PipelineResult
open Functional.ETL.Pipeline
open Functional.ETL.Pipeline.PipelineProcessData

[<Literal>]
let ModuleName = "UndelegateStake"

let action logger tokenSymbol undelegateFrom amountCalcualtor (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
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
            bindCustomJson "tokens" "undelegate" {|from = undelegateFrom;symbol = tokenSymbol;quantity = asStringWithPrecision tokenBalance|}
            |> buildCustomJson username "ssc-mainnet-hive" 
            |> scheduleActiveOperation (logger username) ModuleName tokenSymbol 
            |> withResult entity 
        else 
            TokenBalanceTooLow (ModuleName, tokenSymbol) |> withResult entity
    | _ -> 
        NoUserDetails ModuleName |> withResult entity
let bind logger urls (parameters: Map<string, string>) = 
    let token = parameters.["token"]
    let undelegateFrom = parameters.["undelegateFrom"]
    let amount = parameters.["amount"] |> AmountCalator.bind
    action (logger ModuleName) token undelegateFrom amount
