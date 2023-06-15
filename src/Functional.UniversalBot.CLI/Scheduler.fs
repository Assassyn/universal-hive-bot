module Scheduler

open NCrontab
open NCrontab.Scheduler
open System
open System.Threading
open Configuration
open PipelineResult
open Functional.ETL.Pipeline
open FSharp.Control

let private options = 
    let opt = new CrontabSchedule.ParseOptions ()
    opt.IncludingSeconds <- true
    opt

let private bindAction logger name pipeline = 
     new Action<CancellationToken>(
        fun ct ->   
            processPipeline pipeline 
            |> TaskSeq.iter (fun item -> FinishedProcessing item.index |> logger)
            |> Async.AwaitTask 
            |> Async.RunSynchronously)

let bind logger pipelines =
    let crontab = new Scheduler()

    pipelines
    |> Seq.filter (fun (config, _) -> Readers.selectSchedulerBasedOnly config)
    |> Seq.map (fun (config, pipeline) -> (config.Name, CrontabSchedule.Parse(config.Trigger, options), pipeline))
    |> Seq.map (fun (name, cron, pipeline) -> (cron, bindAction logger name pipeline))
    |> Seq.iter (fun (cron, action) -> crontab.AddTask(cron, action) |> ignore)
    crontab

let private calculateNextOccurance (scheduler: Scheduler) = 
    let occurences = 
        scheduler.GetNextOccurrences ()
        |> Seq.map (fun x -> x.ToTuple())
        |> Seq.map (fun (date, _) -> date)
        |> Seq.sortBy (fun x -> x)
    match occurences |> Seq.length with 
    | 0 -> 
        None
    | _ -> 
        let dateOfOccurence = occurences |> Seq.head
        Some dateOfOccurence

let private createMessageWithNextOccurance scheduler = 
    let dateOfOccurence = calculateNextOccurance scheduler

    if dateOfOccurence.IsSome 
    then 
        dateOfOccurence.Value |> NextOccurance
    else
        Nothing

let start logger (scheduler: Scheduler) = 
    scheduler.Start()

    createMessageWithNextOccurance scheduler |> logger 

    scheduler.Next.Add (fun _ -> createMessageWithNextOccurance scheduler |> logger )
