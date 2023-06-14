module Logging

open Functional.ETL.Pipeline
open PipelineResult
open Lamar

let writeToConsole message = 
    printfn "%O" message

let mutable loggedResults: UniversalHiveBotResutls seq = Seq.empty

let getNotProcessedMessages (results: UniversalHiveBotResutls seq) = 
    results
    |> Seq.filter 
        (fun result -> 
            let contains = loggedResults |> Seq.contains result
            not contains)

let renderResult result =
    match result with 
    | Processed _ ->
        ()
    | _ ->
        writeToConsole result

let logger (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    if entity.results |> Seq.length > 0 
    then
        entity.results.Head
        |> renderResult
        //let notRendered = getNotProcessedMessages entity.results
        
        //notRendered |> Seq.iter renderResult

        //let newLogged =  loggedResults |> Seq.append notRendered
        //loggedResults <- newLo
    entity

let logConfigurationFound (config: Types.Configuration) =
    let actionsMessage = sprintf "Found actions %i to process" (config.actions |> Seq.length)
    writeToConsole actionsMessage

    config.actions
    |> Seq.iter (fun c -> writeToConsole (sprintf "Action name: %s" c.Name))

    writeToConsole "\n"
    config

type ActionRegistry () as self =
    inherit ServiceRegistry ()
    do 
        self.For<Transformer<UniversalHiveBotResutls>>().Use(logger).Named("decorator") |> ignore     
