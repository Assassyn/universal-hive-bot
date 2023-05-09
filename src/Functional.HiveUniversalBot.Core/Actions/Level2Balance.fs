module Level2Balance

open System
open System.Text.Json
open PipelineResult
open Functional.ETL.Pipeline
open FsHttp
open Types
open HiveEngine
    
[<Literal>]
let private ModuleName = "Balance"

let stringAsDecimal (input: string) =
    let mutable number = 0M
    if Decimal.TryParse  (input, &number)
    then 
        number
    else 
        0M
    
let private addTokenBalanceAsProperty entity (tokenInfo: TokenBalance) =
    let tokenBalance = tokenInfo.balance |> stringAsDecimal
    let newEntity = 
        if tokenBalance > 0M 
        then 
            PipelineProcessData.withProperty entity tokenInfo.symbol tokenBalance
        else 
            entity

    let stakeBalance = tokenInfo.stake |> stringAsDecimal
    let newEntity = 
        if stakeBalance > 0M 
        then 
            PipelineProcessData.withProperty newEntity (tokenInfo.symbol+"_stake") stakeBalance
        else 
            newEntity

    newEntity

let action logger hiveEngineUrl (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    let username  = PipelineProcessData.readPropertyAsString entity "username"

    match username with 
    | Some username -> 
        logger ModuleName "Balance" ""
        getBalance hiveEngineUrl username
        |> Seq.fold addTokenBalanceAsProperty entity
    | _ -> 
        NoUserDetails ModuleName 
        |> PipelineProcessData.withResult entity 

let bind logger (urls: Urls) (parameters: Map<string, string>) = 
    action logger urls.hiveEngineNodeUrl
