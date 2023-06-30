module CommentOnPosts

open System
open PipelineResult
open Types 
open Pipeline
open PipelineProcessData
open PostId
open FSharp.Control

[<Literal>]
let ModuleName = "CommentOnPost"

let private filterMessagesWithoutPreviousComment hiveUrl username (post: PostIdentification) = 
    task {
        let! discussion = BridgeAPI.getDiscussion hiveUrl post.author post.permlink
        return  
            discussion
            |> Map.toSeq
            |> Seq.map (fun (key, _) -> key)
            |> Seq.filter (fun key -> key.StartsWith($"{username}/"))
            |> Seq.length = 0
    }

let private createTheComment username body (post: PostIdentification) = 
    let metadata = """{"app":"universalbot/0.10.0"}"""
    Hive.createComment username body metadata post.author post.permlink post.permlink ""

let private getTemplate templateId (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    (Map.getValueWithDefault entity.properties templateId ("" :> obj)).ToString()

let action hiveUrl collectionHandle template username (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    let postToCommentOn =
        enumerateProperties collectionHandle entity
        |> TaskSeq.ofSeq
        |> TaskSeq.filterAsync (filterMessagesWithoutPreviousComment hiveUrl username)
        |> TaskSeq.map (createTheComment username (getTemplate template entity))
        |> TaskSeq.map (Hive.scheduleSinglePostingOperation ModuleName "vote")
        |> TaskSeq.toSeq
    
    let lenght = Seq.length postToCommentOn 
        
    if lenght = 0
    then
        entity |>= CountNotSuccessful "Found no post to comment on"
    else
        postToCommentOn |> Seq.fold withResult entity

let bind urls (parameters: Map<string, string>) = 
    let collectionHandle = Map.getValueWithDefault parameters "label" "posts"
    let template = Map.getValueWithDefault parameters "template" ""

    Action.bindAction ModuleName (action urls.hiveNodeUrl collectionHandle template)
