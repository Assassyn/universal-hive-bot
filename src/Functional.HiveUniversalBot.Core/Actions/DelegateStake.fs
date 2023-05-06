module DelegateStake 

open Core
open PipelineResult
open Functional.ETL.Pipeline

[<Literal>]
let ModuleName = "DelegateStake"

let private getTokenStakeBalance tokensName entity = 
    let key = sprintf "%s_stake" tokensName
    match (PipelineProcessData.readPropertyAsDecimal entity key) with 
    | Some x -> x
    | _ -> 0M

let private buildCustomJson (hive: Hive) username delegateTo tokenSymbol tokenBalance =
    let json =
        sprintf 
            """{"contractName":"tokens","contractAction":"delegate","contractPayload":{"to":"%s","symbol":"%s","quantity":"%M"}}"""
            delegateTo 
            tokenSymbol 
            tokenBalance
    hive.createCustomJsonActiveKey username "ssc-mainnet-hive" json

let private delegateStakeTokens tokenSymbol operation = 
    HiveOperation (ModuleName, tokenSymbol, KeyRequired.Active, operation)
    //let transactionId = hive.brodcastTransaction operation key 
    //Processed (ModuleName, transactionId) 

let action hive tokenSymbol delegateTo amountCalcualtor (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    let userDetails: (string * string * string) option = PipelineProcessData.readPropertyAsType entity "userdata" 

    match userDetails with 
    | Some (username, activeKey, _) -> 
        let tokenBalance = entity |> getTokenStakeBalance tokenSymbol |> amountCalcualtor
        if tokenBalance > 0M
        then 
            let customJson = buildCustomJson hive username delegateTo tokenSymbol tokenBalance
            delegateStakeTokens tokenSymbol customJson |> PipelineProcessData.withResult entity 
        else 
            TokenBalanceTooLow (ModuleName, tokenSymbol) |> PipelineProcessData.withResult entity
    | _ -> 
        NoUserDetails ModuleName |> PipelineProcessData.withResult entity

let bind hive urls (parameters: Map<string, string>) = 
    let token = parameters.["token"]
    let delegateTo = parameters.["delegateTo"]
    let amount = parameters.["amount"] |> AmountCalator.bind
    action hive token delegateTo amount
