module StakeToken 

open Core
open PipelineResult
open Functional.ETL.Pipeline

let private getJson (hive: Hive) tokenStakingDetails =
    let (username, symbol, quantity) = tokenStakingDetails
        
    let json =
        sprintf 
            """{"contractName":"tokens","contractAction":"stake","contractPayload": {"to": "%s","symbol": "%s","quantity": "%M"}}""" 
            username 
            symbol 
            quantity
    let operations = hive.createCustomJsonActiveKey username "ssc-mainnet-hive" json
    (operations, symbol)


let getTokenDetails entity username tokensName = 
    let balance = 
        match (PipelineProcessData.readPropertyAsString entity tokensName) with 
        | Some x -> System.Decimal.Parse x
        | _ -> 0M
    (username, tokensName, balance)

let stakeTokens (hive: Hive) activeKey token operation entity = 
    try 
        let txid = hive.brodcastTransaction operation activeKey
            
        PipelineProcessData.withResult entity (UniversalHiveBotResutls.Processed ("StakeAction", token))
    with  
        | ex -> 
            let msg = ex.Message
            PipelineProcessData.withResult entity (UniversalHiveBotResutls.UnableToProcess ("StakeAction", token, msg))

let ignoreEmpty tokenStakingDetails =
    let (_, _, quantity) = tokenStakingDetails
    quantity > 0.0M 

let action (hive: Hive) tokensToStake (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    let userDetails: (string * string * string) option = PipelineProcessData.readPropertyAsType entity "userdata"

    match userDetails with 
    | Some (username, activeKey, _) -> 
        tokensToStake
        |> Seq.map (getTokenDetails entity username)
        |> Seq.filter ignoreEmpty
        |> Seq.map (getJson hive)
        |> Seq.fold (fun entity (operation, token) -> stakeTokens hive activeKey token operation entity) entity 
    | _ -> 
        PipelineProcessData.withResult entity (UniversalHiveBotResutls.NoUserDetails "StakeAction")

let bind hive urls (parameters: Map<string, string>) = 
    let tokensToStake = parameters.["tokens"].Split(',')
    action hive tokensToStake
