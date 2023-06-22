module Scheduler

open NCrontab
open NCrontab.Scheduler
open System
open System.Threading
open Configuration
open PipelineResult
open Functional.ETL.Pipeline
open FSharp.Control
open System.Threading.Tasks

let private options = 
    let opt = new CrontabSchedule.ParseOptions ()
    opt.IncludingSeconds <- true
    opt

let private bindAction logger name pipeline (cancelationToken: CancellationToken): Task =   
    task {
        return! 
            processPipeline pipeline 
            |> TaskSeq.iter (fun item -> FinishedProcessing item.index |> logger)
    }

let bind logger pipelines =
    let crontab = new Scheduler()

    pipelines
    |> Seq.filter (fun (config, _) -> Readers.selectSchedulerBasedOnly config)
    |> Seq.map (fun (config, pipeline) -> (config.Name, CrontabSchedule.Parse(config.Trigger), pipeline))
    |> Seq.map (fun (name, cron, pipeline) -> (cron, bindAction logger name pipeline))
    |> Seq.map (fun (cron, action) -> new AsyncScheduledTask(cron, action))
    |> Seq.iter crontab.AddTask
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
