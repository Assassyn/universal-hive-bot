module Scheduler

open NCrontab
open NCrontab.Scheduler
open System
open System.Threading
open Configuration
open PipelineResult
open Functional.ETL.Pipeline

let options = 
    let opt = new CrontabSchedule.ParseOptions ()
    opt.IncludingSeconds <- true
    opt

let bind configuration =
    let crontab = new Scheduler()
    createScheduledPipelines configuration
    |> Seq.map (fun (trigger, pipeline) -> (CrontabSchedule.Parse(trigger, options), pipeline))
    |> Seq.map (fun (cron, pipeline) -> (cron, new Action<CancellationToken>(fun ct -> processPipeline pipeline |> ignore)))
    |> Seq.iter (fun (cron, action) -> crontab.AddTask(cron, action) |> ignore)
    crontab

let start (scheduler: Scheduler) = 
    scheduler.Start()
