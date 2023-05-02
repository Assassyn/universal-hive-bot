module Configuration

open Microsoft.Extensions.Configuration
open Types
open Core
open Functional.ETL.Pipeline
open PipelineResult

let getConfiguration () = 
    let config = 
        let config = new ConfigurationBuilder()
        let config = config.AddJsonFile ("configuration.json", true)
        let config = config.AddYamlFile ("configuration.yaml", true)
        let config = config.AddYamlFile ("configuration.yml", true)
        config.Build()

    let actions = config.GetSection("actions").Get<UserActionsDefinition array> ()
    let urls = config.GetSection("urls").Get<Urls>()

    {
        urls = urls
        actions = actions
    }

let private getActionByName (name: string) = 
    match name.ToLower() with 
    | "stake" -> StakeToken.bind
    | "balance" -> Level2Balance.bind
    | _ -> (fun hive url properties -> Transformer.defaultTransformer<PipelineResult.UniversalHiveBotResutls>)

let private bindActions hive url parameters bindingFunctionName =
    let prototypeFunction = (getActionByName bindingFunctionName) 
    let pipelineAction = prototypeFunction hive url parameters
    pipelineAction

let private bindTransfomers hive url (config: UserActionsDefinition) =
    let binder fromConfig = 
        let (bindingFunctionName, parameters ) = fromConfig
        bindActions hive url parameters bindingFunctionName
    config.Tasks
    |> List.ofSeq
    |> List.map (fun item -> (item.Name, item.Parameters |> Seq.map (|KeyValue|)  |> Map.ofSeq))
    |> List.map (fun item -> binder item)
    |> List.fold (fun state next -> state >> next) Transformer.defaultTransformer<PipelineResult.UniversalHiveBotResutls>

let private bindPipeline hive urls (config: UserActionsDefinition) =
    let reader = UserReader.bind [ (config.Username, config.ActiveKey, config.PostingKey) ]
    let transforms = bindTransfomers hive urls config

    Pipeline.bind reader transforms

let createPipelines (config: Configuration) = 
    let hive = Hive (config.urls.hiveNodeUrl)
    let urls = config.urls
    
    config.actions
    |> Seq.map (bindPipeline hive urls)