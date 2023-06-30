module VoteOnPosts

open PipelineResult
open Pipeline
open Types
open PipelineProcessData
open BridgeAPITypes
open System
open PostId
open Types
open PipelineResult
open Pipeline
open PipelineProcessData

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
    let newVotes = 
        enumerateProperties label entity
        |> Seq.filter (checkIfHasBeenCommentedOn username >> not)
        |> Seq.map (castAVote username (getWeigth weight useVaraible entity))
        |> Seq.map (Hive.schedulePostingOperation ModuleName "vote")

    match Seq.length newVotes with 
    | 0 -> 
        entity |>= CountNotSuccessful "Found no post to vote on"
    | _ -> 
        newVotes |> Seq.fold withResult entity

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
