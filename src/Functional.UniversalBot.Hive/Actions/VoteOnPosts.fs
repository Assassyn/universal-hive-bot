﻿module VoteOnPosts

open PipelineResult
open Functional.ETL.Pipeline
open Types
open Functional.ETL.Pipeline.PipelineProcessData
open BridgeAPITypes
open System
open PostId
open Types
open PipelineResult
open Functional.ETL.Pipeline
open Functional.ETL.Pipeline.PipelineProcessData

[<Literal>]
let ModuleName = "VoteOnPost"

let private checkIfHasBeenCommentedOn username (post: PostIdentification) = 
    let posts = post.voters 
    
    posts |> Seq.contains username

let private castAVote username weight (post: PostIdentification) = 
    Hive.createVote username post.author post.permlink weight

let private getWeigth weight useVariable (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    match useVariable with
    | false ->  
        weight
    | true ->
        Map.getValueWithDefault entity.properties "weight" ("10.00":>obj)
        |> Decimal.fromObject
        |> Some.defaultWhenNone 10.00M
        |> fun x -> x * 100M
        |> int16

let action hiveUrl weight useVaraible label username (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    readPropertyAsType entity label
    |> Option.defaultValue Seq.empty
    |> Seq.filter (checkIfHasBeenCommentedOn username >> not)
    |> Seq.map (castAVote username (getWeigth weight useVaraible entity))
    |> Seq.map (Hive.schedulePostingOperation ModuleName "vote")
    |> Seq.fold withResult entity

let bind urls (parameters: Map<string, string>) = 
    let label = Map.getValueWithDefault parameters "label" "posts"
    let weight = 
        Map.getValueWithDefault parameters "weight" ""  
        |> Int16.fromString
        |> Some.defaultWhenNone 1000s
    let useVaraible = 
        Map.getValueWithDefault parameters "useVaraible" ""  
        |> Bool.fromString
        |> Some.defaultWhenNone false
    Action.bindAction ModuleName (action urls.hiveNodeUrl weight useVaraible label)
