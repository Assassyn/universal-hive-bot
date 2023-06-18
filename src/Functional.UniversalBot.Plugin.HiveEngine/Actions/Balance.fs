﻿module Balance

open HiveEngine
open Types
open PipelineResult
open Functional.ETL.Pipeline
open Functional.ETL.Pipeline.PipelineProcessData
open HiveEngineTypes
    
[<Literal>]
let private ModuleName = "Balance"
    
let private addProperty tokenSymbol tokenBalance entity = 
    if tokenBalance > 0M 
    then 
        PipelineProcessData.withProperty entity tokenSymbol tokenBalance
    else 
        entity

let private addTokensDetails tokenDetails entity = 
    PipelineProcessData.withProperty entity TokenInfo.TokenDetailsKey tokenDetails

let private calculateStake tokenInfo pendingUnstakes = 
    let stake = tokenInfo.stake
    let tokenUnstakes: PendingUnstakes seq = 
        pendingUnstakes 
        |> Seq.filter (fun token -> token.symbol = tokenInfo.symbol)
    let quantityLeft = 
        tokenUnstakes
        |> Seq.map (fun x -> x.quantityLeft)
        |> Seq.fold (fun acc next -> acc + next)  0M
    let quantity = 
        tokenUnstakes
        |> Seq.map (fun x -> x.quantity)
        |> Seq.fold (fun acc next -> acc + next)  0M
    stake + quantityLeft - quantity

let private calculateDelegatedStake tokenInfo = 
    let stake = tokenInfo.delegationsIn
    let pendingUnstake =  tokenInfo.pendingUndelegations
    stake - pendingUnstake

let private hasAnyBalance (tokenInfo: TokenBalance) = 
    (tokenInfo.balance > 0M) || (tokenInfo.stake > 0M) || (tokenInfo.delegationsIn > 0M)

let private addTokenBalanceAsProperty pendingUnstakes entity  (tokenInfo: TokenBalance) =
    match hasAnyBalance tokenInfo with 
    | true -> 
        entity
        |> addProperty tokenInfo.symbol tokenInfo.balance
        |> addProperty (tokenInfo.symbol+"_stake") (calculateStake tokenInfo pendingUnstakes)
        |> addProperty (tokenInfo.symbol+"_delegatedstake") (calculateDelegatedStake tokenInfo)
    | _ -> 
        entity

let action hiveEngineUrl username (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    getBalance hiveEngineUrl username
    |> Seq.fold (addTokenBalanceAsProperty (getPendingUnstakes hiveEngineUrl username)) entity
    |> addTokensDetails (getTokenDetails hiveEngineUrl)
    |>= TokenBalanceLoaded username

let bind (urls: Urls) (parameters: Map<string, string>) = 
    Action.bindAction ModuleName (action urls.hiveEngineNodeUrl)
