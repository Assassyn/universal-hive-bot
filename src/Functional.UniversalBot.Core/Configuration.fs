module Configuration

open Microsoft.Extensions.Configuration
open Types
open SeriesToActionsRewriter
open Functional.ETL.Pipeline
open Lamar
open ConfigurationTypes

let getConfiguration () = 
    let config = 
        let config = new ConfigurationBuilder()
        let config = config.AddJsonFile ("configuration.json", true)
        let config = config.AddYamlFile ("configuration.yaml", true)
        let config = config.AddYamlFile ("configuration.yml", true)
        let config = config.AddUserSecrets ("01fcd8b6-1786-4a55-b2e8-6b24e7efaea8", true)
        config.Build()

    let actions = config.GetSection("actions").Get<UserActionsDefinition array> ()
    let urls = config.GetSection("urls").Get<Urls>()
    {
        urls = urls
        actions = actions
    }
    
let private container = 
    new Container (fun service -> 
        service.Scan (fun scanner -> 
            scanner.AssembliesAndExecutablesFromApplicationBaseDirectory ()
            scanner.LookForRegistries ()))

let private getActionByName (name: string) = 
    let binder = container.TryGetInstance<Binder> (name.ToLower ())
    if binder :> obj = null
    then 
        fun urls properties -> (fun entity -> entity)
    else 
        binder

let private getActionDecorator () =
    let actionDecorator = container.TryGetInstance<Transformer<PipelineResult.UniversalHiveBotResutls>> ("decorator")
    if actionDecorator :> obj = null
    then 
        fun entity -> entity
    else 
        actionDecorator

let private bindActions url parameters bindingFunctionName =
    let prototypeFunction = (getActionByName bindingFunctionName) 
    let pipelineAction = prototypeFunction url parameters
    pipelineAction

let private bindTransfomers url (config: UserActionsDefinition) =
    let binder fromConfig = 
        let (bindingFunctionName, parameters ) = fromConfig
        bindActions url parameters bindingFunctionName

    let actionDecorator = getActionDecorator ()

    config.Tasks
    |> Seq.collect splitToActualActionConfigurationItems
    |> List.ofSeq
    |> List.map (fun item -> (item.Name, item.Parameters |> Seq.map (|KeyValue|)  |> Map.ofSeq))
    |> List.map (fun item -> binder item)
    |> List.fold (fun state next -> state >> next >> actionDecorator) Transformer.defaultTransformer<PipelineResult.UniversalHiveBotResutls>

let private bindPipeline urls (config: UserActionsDefinition) =
    let reader = container.GetInstance<UserActionReader>()
    let transforms = bindTransfomers urls config 

    Pipeline.bind (reader [config]) transforms

let createPipelines (config: Configuration)  = 
    let urls = config.urls
    
    config.actions
    |> Seq.map (bindPipeline urls)
