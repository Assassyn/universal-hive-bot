module Level2Balance

open FunctionalString
open FsHttp
open HiveEngine
open Types
open PipelineResult
open System.Text.Json
open Functional.ETL.Pipeline
    
[<Literal>]
let private ModuleName = "Balance"
    
let private addProperty tokenSymbol tokenBalance entity = 
    if tokenBalance > 0M 
    then 
        PipelineProcessData.withProperty entity tokenSymbol tokenBalance
    else 
        entity

let calculateStake hiveEngineUrl tokenInfo = 
    let stake = tokenInfo.stake |> asDecimal
    let getPendingStakes = HiveEngine.getPendingUnstakes hiveEngineUrl tokenInfo.account tokenInfo.symbol
    let quantityLeft = 
        getPendingStakes
        |> Seq.map (fun x -> x.quantityLeft)
        |> Seq.map FunctionalString.asDecimal
        |> Seq.fold (fun acc next -> acc + next)  0M
    let quantity = 
        getPendingStakes
        |> Seq.map (fun x -> x.quantity)
        |> Seq.map FunctionalString.asDecimal
        |> Seq.fold (fun acc next -> acc + next)  0M
    stake + quantityLeft - quantity

let calculateDelegatedStake tokenInfo = 
    let stake = tokenInfo.delegationsIn |> asDecimal
    let pendingUnstake =  tokenInfo.pendingUndelegations |> asDecimal 
    stake - pendingUnstake

let private addTokenBalanceAsProperty hiveEngineUrl entity (tokenInfo: TokenBalance) =
    entity
    |> addProperty tokenInfo.symbol (tokenInfo.balance |> asDecimal)
    |> addProperty (tokenInfo.symbol+"_stake") (calculateStake hiveEngineUrl tokenInfo)
    |> addProperty (tokenInfo.symbol+"_delegatedstake") (calculateDelegatedStake tokenInfo)

let action logger hiveEngineUrl (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    let username  = PipelineProcessData.readPropertyAsString entity "username"

    match username with 
    | Some username -> 
        logger username "Balance"
        getBalance hiveEngineUrl username
        |> Seq.fold (addTokenBalanceAsProperty hiveEngineUrl) entity
    | _ -> 
        NoUserDetails ModuleName 
        |> PipelineProcessData.withResult entity 

let bind logger (urls: Urls) (parameters: Map<string, string>) = 
    action (logger ModuleName) urls.hiveEngineNodeUrl
