module Logging

open Functional.ETL.Pipeline
open PipelineResult
open Lamar

let writeToConsole message = 
    printfn "%O" message

let logger (entity: PipelineProcessData<PipelineResult.UniversalHiveBotResutls>) = 
    if entity.results |> Seq.length > 0 
    then
        let lastMessage = entity.results.Head
        writeToConsole lastMessage
    entity

let logConfigurationFound (config: Types.Configuration) =
    let actionsMessage = sprintf "Found actions %i to process" (config.actions |> Seq.length)
    writeToConsole actionsMessage
    config 

type ActionRegistry () as self =
    inherit ServiceRegistry ()
    do 
        self.For<Transformer<PipelineResult.UniversalHiveBotResutls>>().Use(logger).Named("decorator") |> ignore     
