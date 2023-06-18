module Pipeline

open Microsoft.Extensions.Configuration
open Types
open SeriesToActionsRewriter
open Functional.ETL.Pipeline
open Lamar
open ConfigurationTypes
open System
open FSharp.Control
    
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

let private bindTransfomers url (config: UserActionsDefinition) =
    let binder fromConfig = 
        let (bindingFunctionName, parameters ) = fromConfig
        let action = bindActions url parameters bindingFunctionName
        action 

    config.Tasks
    |> Seq.collect splitToActualActionConfigurationItems
    |> List.ofSeq
    |> List.map (fun item -> (item.Name, item.Parameters |> Seq.map (|KeyValue|)  |> Map.ofSeq))
    |> List.map (fun item -> binder item)
    |> List.map Logging.logingDecorator
    |> TaskSeq.ofList
    
let private bindPipeline urls (config: UserActionsDefinition) =
    let reader = Readers.bindReader config
    let transforms = bindTransfomers urls config 

    let pipeline = Pipeline.bind reader transforms

    (config, pipeline)

let createPipelines (config: Configuration)  = 
    let urls = config.urls
        
    config.actions
    |> Seq.map (bindPipeline urls)
