module TerracoreClaim

open Some
open Hive
open Decimal
open PipelineResult
open TerracoreAPI
open Hive
open Functional.ETL.Pipeline
open Functional.ETL.Pipeline.PipelineProcessData

[<Literal>]
let ModuleName = "TerracoreClaim"
let tokenSymbol = "terraccore_claim" 

let action amountCalcualtor (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    let userDetails: (string * string * string) option = PipelineProcessData.readPropertyAsType entity "userdata" 

    match userDetails with 
    | Some (username, _, _) when username <> "" -> 
        let scrapBalance = 
            TerracoreBalance.scrapHandle
            |> readPropertyAsDecimal entity
            |> defaultWhenNone 0M
            |> amountCalcualtor
            |> roundToPrecision 8
        if scrapBalance > 0M
        then 
            {| ``tx-hash`` = String.generateRandomString 22;amount = String.asString scrapBalance |}
            |> buildCustomJson username "terracore_claim" 
            |> schedulePostingOperation ModuleName tokenSymbol
            |> withResult entity 
        else 
            TokenBalanceTooLow (ModuleName, username, tokenSymbol) |> PipelineProcessData.withResult entity
    | _ -> 
        NoUserDetails ModuleName |> PipelineProcessData.withResult entity

let bind urls (parameters: Map<string, string>) = 
    let amount = parameters.["amount"] |> AmountCalator.bind
    action amount
