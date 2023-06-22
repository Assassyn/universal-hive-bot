module Pipeline

open Microsoft.Extensions.Configuration
open Types
open SeriesToActionsRewriter
open Functional.ETL.Pipeline
open Lamar
open ConfigurationTypes
open System
open FSharp.Control
open PipelineResult
    
[<Literal>]
let private TypeLabel = "type"

[<Literal>]
let private TriggerLabel = "trigger"

let private container = 
    new Container (fun service -> 
        service.Scan (fun scanner -> 
            scanner.AssembliesAndExecutablesFromApplicationBaseDirectory ()
            scanner.LookForRegistries ()))

let private getActionByName (name: string) = 
    let binder = container.TryGetInstance<Binder> (name.ToLower ())
    if binder :> obj = null
    then 
        Binder.defaulBinder
    else 
        binder

let private bindActions url parameters bindingFunctionName =
    let prototypeFunction = (getActionByName bindingFunctionName) 
    let pipelineAction = prototypeFunction url parameters
    pipelineAction

let private bindTransfomers url logingDecorator (config: UserActionsDefinition) =
    let binder fromConfig = 
        let (bindingFunctionName, parameters ) = fromConfig
        let action = bindActions url parameters bindingFunctionName
        action 

    config.Tasks
    |> Seq.collect splitToActualActionConfigurationItems
    |> List.ofSeq
    |> List.map (fun item -> (item.Name, item.Parameters |> Seq.map (|KeyValue|)  |> Map.ofSeq))
    |> List.map (fun item -> binder item)
    |> List.map logingDecorator
    |> TaskSeq.ofList
    
let private bindPipeline urls logingDecorator (config: UserActionsDefinition) =
    let reader = Readers.bindReader config
    let transforms = bindTransfomers urls logingDecorator config 
    
    let settings = Map [ 
        (TypeLabel, config.Type.ToString())
        (TriggerLabel, config.Trigger) ]

    Pipeline.bindCustom config.Name settings reader transforms

let createPipelines logingDecorator (config: Configuration) = 
    let urls = config.urls
        
    config.actions
    |> Seq.map (bindPipeline urls logingDecorator)
    |> TaskSeq.ofSeq


let getType pipeline =
    pipeline.settings.[TypeLabel]

let getTrigger pipeline =
    pipeline.settings.[TriggerLabel]
