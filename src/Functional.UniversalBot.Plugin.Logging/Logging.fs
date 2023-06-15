module Logging

open Serilog
open Microsoft.Extensions.Configuration
open Functional.ETL.Pipeline
open PipelineResult
open Lamar
open System.Runtime.Caching

let private logger = 
    let config  = new ConfigurationBuilder()
    let config = config.AddJsonFile("configuration.json", true)
    let config = config.Build()
    let logger = new LoggerConfiguration()
    let logger = logger.ReadFrom
    let logger = logger.Configuration(config)
    logger.CreateLogger()

let private writeToConsole message = 
    if message <> "" 
    then 
        logger.Information message

let mutable loggedResults: Map<int64, UniversalHiveBotResutls seq> = Map.empty
let cache = new MemoryCache("log")

let private getNotProcessedMessages index (results: UniversalHiveBotResutls seq) = 
    match cache.Contains index with 
    | false -> 
        results
    | _ -> 
        let cachedItems = cache.Get index :?> UniversalHiveBotResutls seq
        let canBeFoundInCache = (cachedItems |> Seq.contains) >> not

        let filtered = 
            results
            |> Seq.filter (fun result -> 
                let contained = cachedItems |> Seq.contains result
                not contained)
        filtered

let renderResult pipelineName result =
    match result with 
    | Processed _ | Nothing -> 
        ()
    | FlushingFinshed _ ->
        writeToConsole $"[{pipelineName}]: Flushing completed"
    | FinishedProcessing index ->
        writeToConsole $"[{pipelineName}]: Flushing processing action"
    | _ -> 
        writeToConsole ($"[{pipelineName}]: {result}")

let logEntity (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    let cacheIndex = entity.index.ToString()
    let pipelineName = entity.properties.[Readers.pipelineName]
    let notRendered = getNotProcessedMessages cacheIndex entity.results |> Array.ofSeq
        
    notRendered |> Seq.iter (renderResult pipelineName)

    let expireIn5Minutes = System.DateTimeOffset.Now.AddMinutes(5)
    cache.Set(cacheIndex, entity.results:> obj, expireIn5Minutes)

    entity

let logConfigurationFound (config: Types.Configuration) =
    let actionsMessage = sprintf "Found actions %i to process" (config.actions |> Seq.length)
    writeToConsole actionsMessage
    config.actions
    |> Seq.iter (fun c -> writeToConsole (sprintf "Action name: %s" c.Name))

    config

type ActionRegistry () as self =
    inherit ServiceRegistry ()
    do 
        self.For<Transformer<UniversalHiveBotResutls>>().Use(logEntity).Named("decorator") |> ignore     
