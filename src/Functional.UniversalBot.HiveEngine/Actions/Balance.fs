module Balance

open HiveEngine
open Types
open PipelineResult
open Functional.ETL.Pipeline
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
    PipelineProcessData.withProperty entity "tokenDetails" tokenDetails

let calculateStake tokenInfo pendingUnstakes = 
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

let calculateDelegatedStake tokenInfo = 
    let stake = tokenInfo.delegationsIn
    let pendingUnstake =  tokenInfo.pendingUndelegations
    stake - pendingUnstake

let private hasAnyBalance (tokenInfo: TokenBalance) = 
    (tokenInfo.balance > 0M) || (tokenInfo.stake > 0M) || (tokenInfo.delegationsIn > 0M)

let private addTokenBalanceAsProperty logger pendingUnstakes entity  (tokenInfo: TokenBalance) =
    match hasAnyBalance tokenInfo with 
    | true -> 
        logger (sprintf "Loading token %s" tokenInfo.symbol)
        entity
        |> addProperty tokenInfo.symbol tokenInfo.balance
        |> addProperty (tokenInfo.symbol+"_stake") (calculateStake tokenInfo pendingUnstakes)
        |> addProperty (tokenInfo.symbol+"_delegatedstake") (calculateDelegatedStake tokenInfo)
    | _ -> 
        entity

let action logger hiveEngineUrl (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    let username  = PipelineProcessData.readPropertyAsString entity "username"

    match username with 
    | Some username -> 
        logger username "Balance"
        getBalance hiveEngineUrl username
        |> Seq.fold (addTokenBalanceAsProperty (logger username) (getPendingUnstakes hiveEngineUrl username)) entity
        |> addTokensDetails (getTokenDetails hiveEngineUrl)
    | _ -> 
        NoUserDetails ModuleName 
        |> PipelineProcessData.withResult entity 

let bind logger (urls: Urls) (parameters: Map<string, string>) = 
    action (logger ModuleName) urls.hiveEngineNodeUrl
