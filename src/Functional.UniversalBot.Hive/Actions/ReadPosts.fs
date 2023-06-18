module ReadPosts

open PipelineResult
open Functional.ETL.Pipeline
open Types
open Functional.ETL.Pipeline.PipelineProcessData
open BridgeAPITypes
open System


let action hiveUrl tag label (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    task {
        let posts = 
            BridgeAPI.getRankedPosts hiveUrl Sort.Created tag
            |> Seq.map PostId.bind
        
        return 
            withProperty entity label posts
            |>= Loaded "ranked_posts"
    }

let bind urls (parameters: Map<string, string>) = 
    let label = Map.getValueWithDefault parameters "label" "posts"
    let tag = Map.getValueWithDefault parameters "tag" ""
    action urls.hiveNodeUrl tag label
