module UndelegateStake 

open PipelineResult
open Functional.ETL.Pipeline

[<Literal>]
let ModuleName = "UndelegateStake"

let private getTokenStakeBalance tokensName entity = 
    let key = sprintf "%s_delegatedstake" tokensName
    match (PipelineProcessData.readPropertyAsDecimal entity key) with 
    | Some x -> x
    | _ -> 0M

let private buildCustomJson username undelegateFrom tokenSymbol tokenBalance =
    let json =
        sprintf 
            """{"contractName":"tokens","contractAction":"undelegate","contractPayload":{"from":"%s","symbol":"%s","quantity":"%M"}}"""
            undelegateFrom 
            tokenSymbol 
            tokenBalance
    Hive.createCustomJsonActiveKey username "ssc-mainnet-hive" json

let private delegateStakeTokens logger tokenSymbol operation = 
    logger tokenSymbol
    HiveOperation (ModuleName, tokenSymbol, KeyRequired.Active, operation)

let action logger tokenSymbol undelegateFrom amountCalcualtor (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    let userDetails: (string * string * string) option = PipelineProcessData.readPropertyAsType entity "userdata" 

    match userDetails with 
    | Some (username, activeKey, _) -> 
        let tokenBalance = entity |> getTokenStakeBalance tokenSymbol |> amountCalcualtor
        if tokenBalance > 0M
        then 
            let customJson = buildCustomJson username undelegateFrom tokenSymbol tokenBalance
            delegateStakeTokens (logger username) tokenSymbol customJson |> PipelineProcessData.withResult entity 
        else 
            TokenBalanceTooLow (ModuleName, tokenSymbol) |> PipelineProcessData.withResult entity
    | _ -> 
        NoUserDetails ModuleName |> PipelineProcessData.withResult entity

let bind logger urls (parameters: Map<string, string>) = 
    let token = parameters.["token"]
    let undelegateFrom = parameters.["undelegateFrom"]
    let amount = parameters.["amount"] |> AmountCalator.bind
    action (logger ModuleName) token undelegateFrom amount
