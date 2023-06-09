module LoadTemplate

open PipelineResult
open Functional.ETL.Pipeline
open Types
open Functional.ETL.Pipeline.PipelineProcessData
open BridgeAPITypes
open System

let private createReplacmentOld = 
    sprintf "{{%s}}"

let private foldProperties (properties: Map<string, obj>) (state: String) next = 
    let mustache = createReplacmentOld next
    state |> String.replaceString mustache (properties.[next].ToString())

let private replaceMustache (properties: Map<string, obj>) (input: string) =
    properties.Keys
    |> Seq.fold (foldProperties properties) input 

let action templateId label username (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    TemplateAPI.getTemplate templateId
    |> replaceMustache entity.properties
    |> withProperty entity label 
    |>= Loaded (sprintf "Template with id %s" templateId)

let bind urls (parameters: Map<string, string>) = 
    let templateId = Map.getValueWithDefault parameters "templateId" ""
    let label = Map.getValueWithDefault parameters "label" "template"
    action templateId label

