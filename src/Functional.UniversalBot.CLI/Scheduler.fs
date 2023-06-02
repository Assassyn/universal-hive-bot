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

let private bindAction logger name pipeline = 
     new Action<CancellationToken>(fun ct -> logger name; processPipeline pipeline |> ignore)

let bind logger configuration  =
    let crontab = new Scheduler()
    createScheduledPipelines configuration
    |> Seq.map (fun (name, trigger, pipeline) -> (name, CrontabSchedule.Parse(trigger, options), pipeline))
    |> Seq.map (fun (name, cron, pipeline) -> (cron, bindAction logger name pipeline))
    |> Seq.iter (fun (cron, action) -> crontab.AddTask(cron, action) |> ignore)
    crontab

let calculateNextOccurance (scheduler: Scheduler) = 
    let occurences = 
        scheduler.GetNextOccurrences ()
        |> Seq.map (fun x -> x.ToTuple())
        |> Seq.map (fun (date, _) -> date)
        |> Seq.sortBy (fun x -> x)
    let dateOfOccurence = occurences |> Seq.head
    dateOfOccurence

let writeMessageWithNextOccurance scheduler = 
    let dateOfOccurence = calculateNextOccurance scheduler
    sprintf "Next action on: %s" (dateOfOccurence.ToString("dd/MM/yyyy HH:mm:ss"))

let start logger (scheduler: Scheduler) = 
    scheduler.Start()
    let message = writeMessageWithNextOccurance scheduler         
    logger message

    scheduler.Next.Add (fun _ -> logger (writeMessageWithNextOccurance scheduler))
