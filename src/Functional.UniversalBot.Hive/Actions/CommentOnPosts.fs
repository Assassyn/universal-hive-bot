module CommentOnPosts

open System
open BridgeAPITypes
open PipelineResult
open Types 
open Functional.ETL.Pipeline.PipelineProcessData
open Functional.ETL.Pipeline

[<Literal>]
let ModuleName = "CommentOnPost"

let private checkIfHasBeenCommentedOn username (post: PostIdentification) = 
    post.voters |> Seq.contains username

let castAVote username weight (post: PostIdentification) = 
    Hive.createVote username post.author post.permlink weight

let action hiveUrl collectionHandle template username (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    readPropertyAsType entity label
    |> Option.defaultValue Seq.empty
    |> Seq.filter (checkIfHasBeenCommentedOn username >> not)
    |> Seq.map (castAVote username weight)
    |> Seq.map (Hive.schedulePostingOperation ModuleName "vote")
    |> Seq.fold withResult entity

let bind urls (parameters: Map<string, string>) = 
    let collectionHandle = Map.getValueWithDefault parameters "label" "posts"
    let template = Map.getValueWithDefault parameters "template" ""
    action urls.hiveNodeUrl tag label
    Action.bindAction ModuleName (action urls.hiveNodeUrl collectionHandle template)
