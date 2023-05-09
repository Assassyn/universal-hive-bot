module SellToken 

open PipelineResult
open Types
open Functional.ETL.Pipeline
open HiveEngine

[<Literal>]
let ModuleName = "Sell"

let private getTokenBalance tokensName entity = 
    match (PipelineProcessData.readPropertyAsDecimal entity tokensName) with 
    | Some x -> x
    | _ -> 0M

let private buildCustomJson  username tokenSymbol tokenBalance price =
    let json =
        sprintf 
            """{"contractName":"market","contractAction":"sell","contractPayload": {"symbol": "%s","quantity": "%M","price":"%M"}}""" 
            tokenSymbol 
            tokenBalance
            price
    Hive.createCustomJsonActiveKey username "ssc-mainnet-hive" json

let private createOperation logger tokenSymbol operation = 
    logger ModuleName tokenSymbol "Scheduled"
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
        let amountToSell = entity |> getTokenBalance tokenSymbol |> amountCalcualtor
        if amountToSell > 0M
        then 
            let tokenPrice = getTokenPrice hiveEngineUrl tokenSymbol amountToSell
            let customJson = buildCustomJson username tokenSymbol amountToSell tokenPrice
            createOperation logger tokenSymbol customJson |> PipelineProcessData.withResult entity 
        else 
            TokenBalanceTooLow (ModuleName, tokenSymbol) |> PipelineProcessData.withResult entity
    | _ -> 
        NoUserDetails ModuleName |> PipelineProcessData.withResult entity

let bind logger (urls: Urls) (parameters: Map<string, string>) = 
    let token = parameters.["token"]
    let amountToSell = parameters.["amountToSell"] |> AmountCalator.bind
    action logger urls.hiveNodeUrl urls.hiveEngineNodeUrl token amountToSell
