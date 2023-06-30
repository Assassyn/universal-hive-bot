module ReadUserComments

open System
open PipelineResult
open Pipeline
open Types
open PipelineProcessData
open BridgeAPITypes

[<Literal>]
let private moduleName = "read_comments"

let private foldPosts label indexEntity post = 
    let (entity, index: Int32) = indexEntity
    let paddedIndex = index.ToString("000")
    let numberedLabel = $"{label}_{paddedIndex}"

    (withProperty entity numberedLabel post), (index + 1)

let action hiveUrl label username (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    BridgeAPI.getAccountComments hiveUrl username 25
    |> Seq.map PostId.bind
    |> Seq.fold (foldPosts label) (entity, 0)
    |> fun (entity, _) -> entity
    |>= Loaded "comments_read"

let bind urls (parameters: Map<string, string>) = 
    let label = Map.getValueWithDefault parameters "label" "comments"
    Action.bindAction moduleName (action urls.hiveNodeUrl label)
