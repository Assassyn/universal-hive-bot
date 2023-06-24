
module Logging

open Microsoft.Extensions.Logging
open System.Runtime.Caching
open PipelineResult
open Functional.ETL.Pipeline

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
        ""
    | FlushingFinshed _ ->
        $"[{pipelineName}]: Flushing completed"
    | FinishedProcessing index ->
        $"[{pipelineName}]: Flushing processing action"
    | _ -> 
        $"[{pipelineName}]: {result}"

let logEntity (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    let cacheIndex = entity.index.ToString()
    let pipelineName = entity.properties.[Readers.pipelineName]
    let notRendered = getNotProcessedMessages cacheIndex entity.results |> Array.ofSeq
    let expireIn5Minutes = System.DateTimeOffset.Now.AddMinutes(5)
    
    cache.Set(cacheIndex, entity.results:> obj, expireIn5Minutes)
    
    notRendered |> Seq.map (renderResult pipelineName)

//let logConfigurationFound (config: Types.Configuration) =
//    let actionsMessage = sprintf "Found actions %i to process" (config.actions |> Seq.length)
//    writeToConsole actionsMessage
//    config.actions
//    |> Seq.iter (fun c -> writeToConsole (sprintf "Action name: %s" c.Name))

//    config

let logingDecorator (logger: ILogger) transformer entity = 
    task {
        let! resultEntity = transformer entity

        resultEntity
        |> logEntity
        |> Seq.iter logger.LogInformation


        return resultEntity
    }
