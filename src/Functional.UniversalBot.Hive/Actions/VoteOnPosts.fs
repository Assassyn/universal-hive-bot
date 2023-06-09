module VoteOnPosts

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
    post.voters |> Seq.contains username

let castAVote username weight (post: PostIdentification) = 
    Hive.createVote username post.author post.permlink weight

let action hiveUrl weight label username (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    readPropertyAsType entity label
    |> Option.defaultValue Seq.empty
    |> Seq.filter (checkIfHasBeenCommentedOn username >> not)
    |> Seq.map (castAVote username weight)
    |> Seq.map (Hive.schedulePostingOperation ModuleName "vote")
    |> Seq.fold withResult entity

let bind urls (parameters: Map<string, string>) = 
    let label = Map.getValueWithDefault parameters "label" "posts"
    let weight = 
        Map.getValueWithDefault parameters "weight" "0"  
        |> Int16.fromString
        |> Some.defaultWhenNone 100s
    Action.bindAction ModuleName (action urls.hiveNodeUrl weight label)
