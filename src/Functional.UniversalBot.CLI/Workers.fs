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

let private areDateEqulaForMinutePrecision (leftDate:DateTime) (rightDate: DateTime) = 
    leftDate.Year = rightDate.Year
    && leftDate.Month = rightDate.Month
    && leftDate.Day = rightDate.Day
    && leftDate.Hour = rightDate.Hour
    && leftDate.Minute = rightDate.Minute

let canExecute timeProvider startTime (pipeline: Pipeline<UniversalHiveBotResutls>) =
    let trigger = pipeline |> getTrigger
    let cron = CrontabSchedule.Parse trigger 
    let nextOccurence = cron.GetNextOccurrence (startTime)
    let now = timeProvider ()
    let areEqual = areDateEqulaForMinutePrecision nextOccurence now
    areEqual

let private delayStart () =
    task {
        do! 
            3
            |> TimeSpan.FromSeconds
            |> Task.Delay
    }

let private getPiplinesByExecution executionType piplines =
    piplines
    |> TaskSeq.filter (fun pipline -> ExecutionType.areEqual executionType (pipline |> getType))
    |> TaskSeq.toArray
    |> TaskSeq.ofArray
    

type ScheduleWorker(config: Types.Configuration, timeProvider: unit -> DateTime, logger: ILogger<ScheduleWorker>) =
    inherit BackgroundService()

    override _.ExecuteAsync(ct: CancellationToken) =
        task {
            let pipelines = 
                createPipelines (Logging.logingDecorator logger) config
                |> getPiplinesByExecution ExecutionType.Scheduler

            let mutable startTime = DateTime.Now

            do! delayStart ()
            while not ct.IsCancellationRequested do
                
                if (areDateEqulaForMinutePrecision startTime DateTime.Now) 
                then 
                    do! 
                        60 
                        |> TimeSpan.FromSeconds
                        |> Task.Delay

                logger.LogDebug("Schedule Worker running at: {time}", DateTimeOffset.Now)
                                
                let results = 
                    pipelines
                    |> TaskSeq.filter (canExecute timeProvider startTime)
                    |> TaskSeq.map processPipeline
                    |> TaskSeq.toArray

                startTime <- DateTime.Now
        }

type ContinousWorker(config: Types.Configuration, logger: ILogger<ContinousWorker>) =
    inherit BackgroundService()
    
    override _.ExecuteAsync(ct: CancellationToken) =
        task {
            let pipelines = 
                createPipelines (Logging.logingDecorator logger) config
                |> getPiplinesByExecution ExecutionType.Continous
                    
            do! delayStart ()
            while not ct.IsCancellationRequested do
                logger.LogDebug ("Continous Worker running at: {time}", DateTimeOffset.Now)
                                    
                let results = 
                    pipelines
                    |> TaskSeq.map processPipeline
                    |> TaskSeq.toArray
                ()
        }
