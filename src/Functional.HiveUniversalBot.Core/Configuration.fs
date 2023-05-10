module Configuration

open Microsoft.Extensions.Configuration
open Types
open Functional.ETL.Pipeline
open PipelineResult

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

let private getActionByName (name: string) = 
    match name.ToLower() with 
    | "stake" -> StakeToken.bind
    | "delegatestake" -> DelegateStake.bind
    | "undelegateStake" -> UndelegateStake.bind
    | "balance" -> Level2Balance.bind
    | "flush" -> FlushTokens.bind
    | "sell" -> SellToken.bind
    | "transfer" -> TransferToken.bind
    | _ -> (fun logger url properties -> Transformer.defaultTransformer<PipelineResult.UniversalHiveBotResutls>)

let private bindActions logger url parameters bindingFunctionName =
    let prototypeFunction = (getActionByName bindingFunctionName) 
    let pipelineAction = prototypeFunction logger url parameters
    pipelineAction

let private bindTransfomers logger url (config: UserActionsDefinition) =
    let binder fromConfig = 
        let (bindingFunctionName, parameters ) = fromConfig
        bindActions logger url parameters bindingFunctionName
    config.Tasks
    |> List.ofSeq
    |> List.map (fun item -> (item.Name, item.Parameters |> Seq.map (|KeyValue|)  |> Map.ofSeq))
    |> List.map (fun item -> binder item)
    |> List.fold (fun state next -> state >> next) Transformer.defaultTransformer<PipelineResult.UniversalHiveBotResutls>

let private bindPipeline logger urls (config: UserActionsDefinition) =
    let reader = UserReader.bind [ (config.Username, config.ActiveKey, config.PostingKey) ]
    let transforms = bindTransfomers logger urls config

    Pipeline.bind reader transforms

let createPipelines (config: Configuration) logger = 
    let urls = config.urls
    
    config.actions
    |> Seq.map (bindPipeline logger urls)
