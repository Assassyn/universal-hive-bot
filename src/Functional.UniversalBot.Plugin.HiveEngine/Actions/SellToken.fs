﻿module SellToken 

open Action
open Hive
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
        |> Seq.find (fun marketBook -> marketBook.quantity >= quantityToSell)
    priceItem.price

let action hive hiveEngineUrl tokenSymbol amountCalcualtor username (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    let amountToSell = 
        tokenSymbol
        |> readPropertyAsDecimal entity
        |> defaultWhenNone 0M
        |> amountCalcualtor

    if amountToSell > 0M
    then 
        let tokenPrice = getTokenPrice hiveEngineUrl tokenSymbol amountToSell
        bindCustomJson "market" "sell" {| symbol = tokenSymbol; quantity = String.asString amountToSell; price = String.asString tokenPrice; |}
        |> buildActiveKeyedCustomJson username "ssc-mainnet-hive"
        |> scheduleActiveOperation ModuleName tokenSymbol
        |> withResult entity
    else 
        TokenBalanceTooLow (ModuleName, username, tokenSymbol) |> PipelineProcessData.withResult entity

let bind (urls: Urls) (parameters: Map<string, string>) = 
    let token = parameters.["token"]
    let amountToSell = parameters.["amountToSell"] |> AmountCalator.bind
    Action.bindAction ModuleName (action urls.hiveNodeUrl urls.hiveEngineNodeUrl token amountToSell)
