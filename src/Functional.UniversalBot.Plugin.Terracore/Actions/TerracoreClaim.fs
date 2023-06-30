module TerracoreClaim

open Action
open Some
open Hive
open Decimal
open PipelineResult
open TerracoreAPI
open Hive
open Pipeline
open PipelineProcessData

[<Literal>]
let ModuleName = "TerracoreClaim"
let tokenSymbol = "terraccore_claim" 

let action amountCalcualtor username (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    let scrapBalance = 
        TerracoreBalance.scrapHandle
        |> readPropertyAsDecimal entity
        |> defaultWhenNone 0M
        |> amountCalcualtor
        |> roundToPrecision 8
    if scrapBalance > 0M
    then 
        {| ``tx-hash`` = String.generateRandomString 22;amount = String.asString scrapBalance |}
        |> buildPostingKeyedCustomJson username "terracore_claim" 
        |> schedulePostingOperation ModuleName tokenSymbol
        |> withResult entity 
    else 
        TokenBalanceTooLow (ModuleName, username, tokenSymbol) |> PipelineProcessData.withResult entity

let bind urls (parameters: Map<string, string>) = 
    let amount = parameters.["amount"] |> AmountCalator.bind
    Action.bindAction ModuleName (action amount)
