module CommentOnPosts

open System
open PipelineResult
open Types 
open Functional.ETL.Pipeline.PipelineProcessData
open Functional.ETL.Pipeline
open PostId

[<Literal>]
let ModuleName = "CommentOnPost"

let private checkForPreviousComments hiveUrl username (post: PostIdentification) = 
    let posts = 
        BridgeAPI.getDiscussion hiveUrl post.author post.permlink
        |> Map.toSeq
        |> Seq.map (fun (key, _) -> key)
        |> Seq.filter (fun key -> key.StartsWith($"{username}/"))
        |> Seq.length > 0
        
    posts 

let private createTheComment username body (post: PostIdentification) = 
    let metadata = """{"app":"universalbot/0.10.0"}"""
    Hive.createComment username body metadata post.author post.permlink post.permlink ""

let private getTemplate templateId (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    (Map.getValueWithDefault entity.properties templateId ("" :> obj)).ToString()

let action hiveUrl collectionHandle template username (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    let postToCommentOn =
        readPropertyAsType entity collectionHandle
        |> Option.defaultValue Seq.empty
        |> Seq.filter (checkForPreviousComments hiveUrl username >> not)
        |> Seq.map (createTheComment username (getTemplate template entity))
        |> Seq.map (Hive.scheduleSinglePostingOperation ModuleName "vote")
    
    match Seq.length postToCommentOn with 
    | 0 -> 
        entity |>= CountNotSuccessful "Found no post to comment on"
    | _ -> 
        postToCommentOn |> Seq.fold withResult entity

let bind urls (parameters: Map<string, string>) = 
    let collectionHandle = Map.getValueWithDefault parameters "label" "posts"
    let template = Map.getValueWithDefault parameters "template" ""

    Action.bindAction ModuleName (action urls.hiveNodeUrl collectionHandle template)
