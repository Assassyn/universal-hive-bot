﻿module StakeToken 

open Core
open PipelineResult
open Functional.ETL.Pipeline

[<Literal>]
let ModuleName = "Stake"

let private getTokenBalance tokensName entity = 
    match (PipelineProcessData.readPropertyAsDecimal entity tokensName) with 
    | Some x -> x
    | _ -> 0M

let private buildCustomJson (hive: Hive) username tokenSymbol tokenBalance =
    let json =
        sprintf 
            """{"contractName":"tokens","contractAction":"stake","contractPayload": {"to": "%s","symbol": "%s","quantity": "%M"}}""" 
            username 
            tokenSymbol 
            tokenBalance
    hive.createCustomJsonActiveKey username "ssc-mainnet-hive" json

let private stakeTokens logger tokenSymbol operation = 
    logger ModuleName tokenSymbol "Scheduled"
    HiveOperation (ModuleName, tokenSymbol, KeyRequired.Active, operation)

let action logger hive tokenSymbol amountCalcualtor (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    let userDetails: (string * string * string) option = PipelineProcessData.readPropertyAsType entity "userdata" 

    match userDetails with 
    | Some (username, _, _) -> 
        let tokenBalance = entity |> getTokenBalance tokenSymbol |> amountCalcualtor
        if tokenBalance > 0M
        then 
            let customJson = buildCustomJson hive username tokenSymbol tokenBalance
            stakeTokens logger tokenSymbol customJson |> PipelineProcessData.withResult entity 
        else 
            TokenBalanceTooLow (ModuleName, tokenSymbol) |> PipelineProcessData.withResult entity
    | _ -> 
        NoUserDetails ModuleName |> PipelineProcessData.withResult entity

let bind logger hive urls (parameters: Map<string, string>) = 
    let token = parameters.["token"]
    let amount = parameters.["amount"] |> AmountCalator.bind
    action logger hive token amount
