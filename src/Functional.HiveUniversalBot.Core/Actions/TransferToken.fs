module TransferToken 

open PipelineResult
open Some
open Functional.ETL.Pipeline
open Functional.ETL.Pipeline.PipelineProcessData

[<Literal>]
let ModuleName = "Transfer"

let private buildCustomJson username delegateTo tokenSymbol tokenBalance memo =
    let json =
        sprintf 
            """{"contractName":"tokens","contractAction":"transfer","contractPayload":{"to":"%s","symbol":"%s","quantity":"%M","memo":"%s"}}"""
            delegateTo 
            tokenSymbol 
            tokenBalance
            memo
    Hive.createCustomJsonActiveKey username "ssc-mainnet-hive" json

let private delegateStakeTokens logger tokenSymbol operation = 
    logger tokenSymbol
    HiveOperation (ModuleName, tokenSymbol, KeyRequired.Active, operation)

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
            let customJson = buildCustomJson username transferTo tokenSymbol tokenBalance memo
            delegateStakeTokens (logger username) tokenSymbol customJson |> PipelineProcessData.withResult entity 
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
