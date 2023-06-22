module BackgroundTaskRunner

open FSharp.Control
open Functional.ETL.Pipeline
open PipelineResult

let runPipeline logger pipeline = 
    task {
        return! 
            processPipeline pipeline 
            |> TaskSeq.iter (fun item -> FinishedProcessing item.index |> logger)
    }

let start logger pipelines = 
    pipelines
    //|> Seq.filter (fun (config, _) -> Readers.selectContinouseBasedOnly config)
    |> TaskSeq.ofSeq
    |> TaskSeq.iterAsync (fun (_, pipeline) -> runPipeline logger pipeline)
