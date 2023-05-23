module TransferToken 

open Action
open PipelineResult
open Some
open Functional.ETL.Pipeline
open Functional.ETL.Pipeline.PipelineProcessData

[<Literal>]
let ModuleName = "Transfer"

let action logger tokenSymbol transferTo amountCalcualtor memo (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    let userDetails: (string * string * string) option = PipelineProcessData.readPropertyAsType entity "userdata" 

    match userDetails with 
    | Some (username, _, _) when username <> "" -> 
        let tokenBalance = 
            readPropertyAsDecimal entity tokenSymbol 
            |> defaultWhenNone 0M
            |> amountCalcualtor

        if tokenBalance > 0M
        then 
            bindCustomJson "tokens" "transfer" {| ``to`` = username;symbol = tokenSymbol;quantity = String.asString tokenBalance; memo = memo|}
            |> buildCustomJson username "ssc-mainnet-hive" 
            |> scheduleActiveOperation (logger username) ModuleName tokenSymbol 
            |> withResult entity 
        else 
            TokenBalanceTooLow (ModuleName, tokenSymbol) |> PipelineProcessData.withResult entity
    | _ -> 
        NoUserDetails ModuleName |> PipelineProcessData.withResult entity

let bind logger  urls (parameters: Map<string, string>) = 
    let token = parameters.["token"]
    let transferTo = parameters.["transferTo"]
    let memo = Map.getValueWithDefault parameters "memo" ""
    let amount = parameters.["amount"] |> AmountCalator.bind
    action (logger ModuleName) token transferTo amount memo
