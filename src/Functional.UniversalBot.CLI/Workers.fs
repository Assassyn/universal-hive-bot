module Workers 

open System
open System.Threading
open System.Threading.Tasks
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Pipeline
open PipelineResult
open Functional.ETL.Pipeline
open NCrontab
open Types
open FSharp.Control

type Worker(config: Types.Configuration, logger: ILogger<Worker>) =
    inherit BackgroundService()


    let canExecute startTime (pipeline: Pipeline<UniversalHiveBotResutls>) =
        match pipeline with 
        | x when x |> getType = "Continous" -> 
            true
        | x when x |> getTrigger <> "" -> 
            let trigger = x |> getTrigger
            let cron = CrontabSchedule.Parse trigger
            let nextOccurence = cron.GetNextOccurrence (startTime)
            let now = DateTime.Now 
            let diffrence = nextOccurence - now

            diffrence.TotalSeconds < 60
        | _ ->
            false

    override _.ExecuteAsync(ct: CancellationToken) =
        task {
            let pipelines = 
                createPipelines (Logging.logingDecorator logger) config
                
            let mutable startTime = DateTime.Now

            do! 
                3
                |> TimeSpan.FromSeconds
                |> Task.Delay

            while not ct.IsCancellationRequested do
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)
                                
                let results = 
                    pipelines
                    |> TaskSeq.filter (canExecute startTime)
                    |> TaskSeq.map processPipeline
                    |> TaskSeq.toArray

                startTime <- DateTime.Now
        }
