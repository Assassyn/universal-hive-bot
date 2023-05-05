﻿module StakeToken 

open Core
open PipelineResult
open Functional.ETL.Pipeline

[<Literal>]
let private ModuleName = "Stake"

let private getTokenBalance tokensName entity = 
    match (PipelineProcessData.readPropertyAsString entity tokensName) with 
    | Some x -> System.Decimal.Parse x
    | _ -> 0M

let private buildCustomJson (hive: Hive) username tokenSymbol tokenBalance =
    let json =
        sprintf 
            """{"contractName":"tokens","contractAction":"stake","contractPayload": {"to": "%s","symbol": "%s","quantity": "%M"}}""" 
            username 
            tokenSymbol 
            tokenBalance
    hive.createCustomJsonActiveKey username "ssc-mainnet-hive" json

let private requestTokenStakeProcess operation entity = 
    HiveOperation (ModuleName, KeyRequired.Active, operation)
    |> PipelineProcessData.withResult entity 

let action (hive: Hive) tokenSymbol amountCalcualtor (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    let userDetails: (string * string * string) option = PipelineProcessData.readPropertyAsType entity "userdata"

    match userDetails with 
    | Some (username, _, _) -> 
        let tokenBalance = entity |> getTokenBalance tokenSymbol |> amountCalcualtor
        if tokenBalance > 0M
        then 
            let customJson = buildCustomJson hive username tokenSymbol tokenBalance
            entity |> requestTokenStakeProcess customJson 
        else 
            TokenBalanceTooLow (ModuleName, tokenSymbol) |> PipelineProcessData.withResult entity
    | _ -> 
        NoUserDetails ModuleName |> PipelineProcessData.withResult entity

let bind hive urls (parameters: Map<string, string>) = 
    let token = parameters.["token"]
    let amount = parameters.["amount"] |> AmountCalator.bind
    action hive token amount
