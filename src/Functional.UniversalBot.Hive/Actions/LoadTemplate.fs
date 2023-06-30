module LoadTemplate

open PipelineResult
open Pipeline
open Types
open PipelineProcessData
open BridgeAPITypes
open System

[<Literal>]
let private ModuleName = "LoadTemplate" 

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
    Action.bindAction ModuleName (action templateId label)

