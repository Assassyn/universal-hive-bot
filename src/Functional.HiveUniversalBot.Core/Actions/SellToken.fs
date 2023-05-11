module SellToken 

open HiveEngine
open PipelineResult
open Some
open Types
open Functional.ETL.Pipeline
open Functional.ETL.Pipeline.PipelineProcessData

[<Literal>]
let ModuleName = "Sell"

let private buildCustomJson  username tokenSymbol tokenBalance price =
    let json =
        sprintf 
            """{"contractName":"market","contractAction":"sell","contractPayload": {"symbol": "%s","quantity": "%M","price":"%M"}}""" 
            tokenSymbol 
            tokenBalance
            price
    Hive.createCustomJsonActiveKey username "ssc-mainnet-hive" json

let private createOperation logger tokenSymbol operation = 
    logger tokenSymbol
    HiveOperation (ModuleName, tokenSymbol, KeyRequired.Active, operation)

let private getTokenPrice hiveEngineUrl tokenSymbol quantityToSell = 
    let priceItem =
        getMarketBuyBook hiveEngineUrl tokenSymbol
        |> Seq.find (fun marketBook -> (marketBook.quantity |> String.asDecimal) >= quantityToSell)
    priceItem.price |> String.asDecimal

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
            let customJson = buildCustomJson username tokenSymbol amountToSell tokenPrice
            createOperation (logger username) tokenSymbol customJson |> PipelineProcessData.withResult entity 
        else 
            TokenBalanceTooLow (ModuleName, tokenSymbol) |> PipelineProcessData.withResult entity
    | _ -> 
        NoUserDetails ModuleName |> PipelineProcessData.withResult entity

let bind logger (urls: Urls) (parameters: Map<string, string>) = 
    let token = parameters.["token"]
    let amountToSell = parameters.["amountToSell"] |> AmountCalator.bind
    action (logger ModuleName) urls.hiveNodeUrl urls.hiveEngineNodeUrl token amountToSell
