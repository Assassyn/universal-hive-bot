module Configuration

open Microsoft.Extensions.Configuration
open Types
open SeriesToActionsRewriter
open Functional.ETL.Pipeline
open Lamar.Scanning.Conventions
open Lamar

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
    
type Loger = string -> string -> string -> unit
type Binder = Loger -> Urls -> Map<string, string> -> Transformer<PipelineResult.UniversalHiveBotResutls>
type ActinoRegistry () as self =
    inherit ServiceRegistry ()
    do 
        let defaultBinder = (fun logger url properties -> Transformer.defaultTransformer<PipelineResult.UniversalHiveBotResutls>)
        self.For<Binder>().Use(StakeToken.bind).Named("stake") |> ignore
        self.For<Binder>().Use(UnstakeToken.bind).Named("unstake") |> ignore
        self.For<Binder>().Use(DelegateStake.bind).Named("delegatestake") |> ignore
        self.For<Binder>().Use(UndelegateStake.bind).Named("undelegateStake") |> ignore
        self.For<Binder>().Use(Level2Balance.bind).Named("balance") |> ignore
        self.For<Binder>().Use(FlushTokens.bind).Named("flush") |> ignore
        self.For<Binder>().Use(SellToken.bind).Named("sell") |> ignore
        self.For<Binder>().Use(TransferToken.bind).Named("transfer") |> ignore
        self.For<Binder>().Use(AddTokenToPool.bind).Named("addtopool") |> ignore
        self.For<Binder>().Use(FlushAndBalanceAction.bind).Named("flushandbalance") |> ignore
        self.For<Binder>().Use(TokenSwapAction.bind).Named("swaptoken") |> ignore     
        self.For<Binder>().UseIfNone(defaultBinder) 

        

let private container = 
    new Lamar.Container (fun service -> 
        service.Scan (fun scanner -> 
            scanner.AssembliesFromApplicationBaseDirectory ()
            scanner.LookForRegistries ()))


let private getActionByName (name: string) = 
    let action = container.GetInstance<Binder> (name.ToLower ())
    action

let private bindActions logger url parameters bindingFunctionName =
    let prototypeFunction = (getActionByName bindingFunctionName) 
    let pipelineAction = prototypeFunction logger url parameters
    pipelineAction

let private bindTransfomers logger url (config: UserActionsDefinition) =
    let binder fromConfig = 
        let (bindingFunctionName, parameters ) = fromConfig
        bindActions logger url parameters bindingFunctionName
    config.Tasks
    |> Seq.collect splitToActualActionConfigurationItems
    |> List.ofSeq
    |> List.map (fun item -> (item.Name, item.Parameters |> Seq.map (|KeyValue|)  |> Map.ofSeq))
    |> List.map (fun item -> binder item)
    |> List.fold (fun state next -> state >> next) Transformer.defaultTransformer<PipelineResult.UniversalHiveBotResutls>

let private bindPipeline logger urls (config: UserActionsDefinition) =
    let test = container.GetAllInstances<Binder> ();
    let reader = UserReader.bind [ (config.Username, config.ActiveKey, config.PostingKey) ]
    let transforms = bindTransfomers logger urls config

    Pipeline.bind reader transforms

let createPipelines (config: Configuration) logger = 
    let urls = config.urls
    
    config.actions
    |> Seq.map (bindPipeline logger urls)
