module UnstakeToken

open PipelineResult
open Some
open Functional.ETL.Pipeline
open Functional.ETL.Pipeline.PipelineProcessData

[<Literal>]
let ModuleName = "Unstake"

let private buildCustomJson  username tokenSymbol tokenBalance =
    let json =
        sprintf 
            """{"contractName":"tokens","contractAction":"unstake","contractPayload":{"symbol":"%s","quantity":"%M"}}""" 
            tokenSymbol 
            tokenBalance
    Hive.createCustomJsonActiveKey username "ssc-mainnet-hive" json

let private unstakeTokens logger tokenSymbol operation = 
    logger tokenSymbol
    HiveOperation (ModuleName, tokenSymbol, KeyRequired.Active, operation)

let action logger tokenSymbol amountCalcualtor (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    let userDetails: (string * string * string) option = PipelineProcessData.readPropertyAsType entity "userdata" 

    match userDetails with 
    | Some (username, _, _) -> 
        let tokenBalance = 
            sprintf "%s_stake" tokenSymbol
            |> readPropertyAsDecimal entity
            |> defaultWhenNone 0M
            |> amountCalcualtor
        if tokenBalance > 0M
        then 
            let customJson = buildCustomJson username tokenSymbol tokenBalance
            unstakeTokens (logger username) tokenSymbol customJson |> PipelineProcessData.withResult entity 
        else 
            TokenBalanceTooLow (ModuleName, tokenSymbol) |> PipelineProcessData.withResult entity
    | _ -> 
        NoUserDetails ModuleName |> PipelineProcessData.withResult entity

let bind logger urls (parameters: Map<string, string>) = 
    let token = parameters.["token"]
    let amount = parameters.["amount"] |> AmountCalator.bind
    action (logger ModuleName) token amount
