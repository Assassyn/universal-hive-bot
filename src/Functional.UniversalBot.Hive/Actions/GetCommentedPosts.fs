module GetCommentedPosts

open System
open PipelineResult
open Pipeline
open Types
open PipelineProcessData
open BridgeAPITypes
open PostId

[<Literal>]
let private moduleName = "get_commented_post"

let private foldPosts label indexEntity post = 
    let (entity, index: Int32) = indexEntity
    let paddedIndex = index.ToString("000")
    let numberedLabel = $"{label}_{paddedIndex}"

    (withProperty entity numberedLabel post), (index + 1)
let action hiveUrl label outputLabel username (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    entity 
    |> enumerateProperties label
    |> Seq.map (fun post -> post :> PostIdentification)
    |> Seq.map (fun post -> BridgeAPI.getPost hiveUrl post.parent_author post.parent_permlink)
    |> Seq.map PostId.bindPost
    |> Seq.fold (foldPosts outputLabel) (entity, 0)
    |> fun (entity, _) -> entity
    |>= Loaded "parent_posts"

let bind urls (parameters: Map<string, string>) = 
    let label = Map.getValueWithDefault parameters "label" "items"
    let outputLabel = Map.getValueWithDefault parameters "parentPostLabel" "parent_items"
    Action.bindAction moduleName (action urls.hiveNodeUrl label outputLabel)
