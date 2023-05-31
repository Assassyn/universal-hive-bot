module Logging

open Functional.ETL.Pipeline
open PipelineResult
open Lamar

let logger (entity: PipelineProcessData<PipelineResult.UniversalHiveBotResutls>) = 
    if entity.results |> Seq.length > 0 
    then
        let lastMessage = entity.results.Head
        printfn "%O" lastMessage
    entity

type ActionRegistry () as self =
    inherit ServiceRegistry ()
    do 
        self.For<Transformer<PipelineResult.UniversalHiveBotResutls>>().Use(logger).Named("decorator") |> ignore     
