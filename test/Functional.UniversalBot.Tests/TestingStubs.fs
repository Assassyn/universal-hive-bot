module TestingStubs

open PipelineResult
open Functional.ETL.Pipeline
open Functional.ETL.Pipeline.PipelineProcessData
open FSharp.Control
open Types
open HiveEngineTypes
open System.Collections.Generic

let a b =
    ()

let pipelineLogger a b c = 
    ()

let mockedBalanceAction balanceLevles entity = 
    balanceLevles
    |> Seq.fold (fun entity (tokenSymbol, tokenBalance) -> PipelineProcessData.withProperty entity tokenSymbol tokenBalance) entity
    |> addProperty TokenInfo.TokenDetailsKey Seq.empty<TokenInfo>

let mockedStakedBalanceAction balanceLevles entity = 
    let stakedTokenSymbole = sprintf "%s_stake"
    balanceLevles
    |> Seq.fold (fun entity (tokenSymbol, tokenBalance) -> PipelineProcessData.withProperty entity (stakedTokenSymbole tokenSymbol) tokenBalance) entity
    |> addProperty TokenInfo.TokenDetailsKey Seq.empty<TokenInfo>
    
let mockedDelegatedStakedBalanceAction balanceLevles entity = 
    let stakedTokenSymbole = sprintf "%s_delegatedstake"
    balanceLevles
    |> Seq.fold (fun entity (tokenSymbol, tokenBalance) -> PipelineProcessData.withProperty entity (stakedTokenSymbole tokenSymbol) tokenBalance) entity
    |> addProperty TokenInfo.TokenDetailsKey Seq.empty<TokenInfo>

let mockedTerracoreBalanceAction tokenBalance entity = 
    PipelineProcessData.withProperty entity TerracoreBalance.scrapHandle tokenBalance
    |> addProperty TokenInfo.TokenDetailsKey Seq.empty<TokenInfo>
    
let extractCustomJson underTestObject =
    match underTestObject with 
    //| PipelineResult.HiveOperation (_, _, _, customJson) -> customJson.json
    | _ -> ""

let inline (~~) x = x :> obj

let reader: unit -> PipelineProcessData<UniversalHiveBotResutls> taskSeq =
    let userDefinition = new UserActionsDefinition ()
    userDefinition.Username <- "universal-bot"
    userDefinition.ActiveKey <- ""
    userDefinition.PostingKey <- ""

    Readers.bindReader userDefinition

let noUserReader: unit -> PipelineProcessData<UniversalHiveBotResutls> taskSeq = 
    let userDefinition = new UserActionsDefinition ()
    userDefinition.Username <- ""
    userDefinition.ActiveKey <- ""
    userDefinition.PostingKey <- ""
    
    Readers.bindReader userDefinition

let bindAmount amount =
    amount |> AmountCalator.bind
