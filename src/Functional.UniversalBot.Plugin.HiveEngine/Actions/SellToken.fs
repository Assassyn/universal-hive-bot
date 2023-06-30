module SellToken 

open Action
open Hive
open HiveEngine
open Some
open Types
open PipelineResult
open Pipeline
open PipelineProcessData

[<Literal>]
let ModuleName = "Sell"

let private getTokenPrice hiveEngineUrl tokenSymbol quantityToSell = 
    let priceItem =
        getMarketBuyBook hiveEngineUrl tokenSymbol
        |> Seq.tryFind (fun marketBook -> marketBook.quantity >= quantityToSell)

    match priceItem with 
    | Some item -> 
        item.price
    | _ -> 
        0M

let action hive hiveEngineUrl tokenSymbol amountCalcualtor username (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    let amountToSell = 
        tokenSymbol
        |> readPropertyAsDecimal entity
        |> defaultWhenNone 0M
        |> amountCalcualtor

    if amountToSell > 0M
    then 
        let tokenPrice = getTokenPrice hiveEngineUrl tokenSymbol amountToSell

        match tokenPrice with 
        | 0M -> 
            ValueTooLow (ModuleName, tokenSymbol, Value.Price) 
            |> withResult entity
        | _ -> 
            bindCustomJson "market" "sell" {| symbol = tokenSymbol; quantity = String.asString amountToSell; price = String.asString tokenPrice; |}
            |> buildActiveKeyedCustomJson username "ssc-mainnet-hive"
            |> scheduleActiveOperation ModuleName tokenSymbol
            |> withResult entity
    else 
        ValueTooLow (ModuleName, tokenSymbol, Value.Balance) 
        |> withResult entity

let bind (urls: Urls) (parameters: Map<string, string>) = 
    let token = parameters.["token"]
    let amountToSell = parameters.["amountToSell"] |> AmountCalator.bind
    Action.bindAction ModuleName (action urls.hiveNodeUrl urls.hiveEngineNodeUrl token amountToSell)
