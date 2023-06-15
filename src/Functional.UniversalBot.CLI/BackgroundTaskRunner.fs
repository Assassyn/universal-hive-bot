module BackgroundTaskRunner

open FSharp.Control
open Functional.ETL.Pipeline
open PipelineResult

let runPipeline logger pipeline = 
    processPipeline pipeline 
    |> TaskSeq.iter (fun item -> FinishedProcessing item.index |> logger)
    |> Async.AwaitTask 
    |> Async.RunSynchronously

let start logger pipelines = 
    pipelines
    |> Seq.filter (fun (config, _) -> Readers.selectContinouseBasedOnly config)
    |> Seq.iter (fun (_, pipeline) -> runPipeline logger pipeline)
