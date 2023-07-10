module WritePosts

open System
open PipelineResult
open Pipeline
open Types
open PipelineProcessData
open BridgeAPITypes

let private foldPosts label indexEntity post = 
    let (entity, index: Int32) = indexEntity
    let paddedIndex = index.ToString("000")
    let numberedLabel = $"{label}_{paddedIndex}"

    (withProperty entity numberedLabel post), (index + 1)

let action hiveUrl tag label (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    task {
        return 
            BridgeAPI.getRankedPosts hiveUrl Sort.Created tag
            |> Seq.map PostId.bind
            |> Seq.fold (foldPosts label) (entity, 0)
            |> fun (entity, _) -> entity
            |>= Loaded "ranked_posts"
    }

let bind urls (parameters: Map<string, string>) = 
    let label = Map.getValueWithDefault parameters "label" "posts"
    let tag = Map.getValueWithDefault parameters "tag" ""
    action urls.hiveNodeUrl tag label
