module SellToken 

open Action
open FunctionalString
open HiveEngine
open Some
open Types
open PipelineResult
open Functional.ETL.Pipeline
open Functional.ETL.Pipeline.PipelineProcessData

[<Literal>]
let ModuleName = "Sell"

let private getTokenPrice hiveEngineUrl tokenSymbol quantityToSell = 
    let priceItem =
        getMarketBuyBook hiveEngineUrl tokenSymbol
        |> Seq.find (fun marketBook -> (marketBook.quantity |> asDecimal) >= quantityToSell)
    priceItem.price |> asDecimal

let action logger hive hiveEngineUrl tokenSymbol amountCalcualtor (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    let userDetails: (string * string * string) option = PipelineProcessData.readPropertyAsType entity "userdata" 

    match userDetails with 
    | Some (username, _, _) -> 
        let amountToSell = 
            tokenSymbol
            |> readPropertyAsDecimal entity
            |> defaultWhenNone 0M
            |> amountCalcualtor

        if amountToSell > 0M
        then 
            let tokenPrice = getTokenPrice hiveEngineUrl tokenSymbol amountToSell
            bindCustomJson "market" "sell" {| symbol = tokenSymbol; quantity = asString amountToSell; price = asString tokenPrice; |}
            |> buildCustomJson username "ssc-mainnet-hive"
            |> scheduleActiveOperation (logger username) ModuleName tokenSymbol
            |> withResult entity
        else 
            TokenBalanceTooLow (ModuleName, tokenSymbol) |> PipelineProcessData.withResult entity
    | _ -> 
        NoUserDetails ModuleName |> PipelineProcessData.withResult entity

let bind logger (urls: Urls) (parameters: Map<string, string>) = 
    let token = parameters.["token"]
    let amountToSell = parameters.["amountToSell"] |> AmountCalator.bind
    action (logger ModuleName) urls.hiveNodeUrl urls.hiveEngineNodeUrl token amountToSell
