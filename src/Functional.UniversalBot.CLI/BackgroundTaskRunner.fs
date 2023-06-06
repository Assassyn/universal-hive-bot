module BackgroundTaskRunner

open FSharp.Control
open Functional.ETL.Pipeline

let runPipeline logger pipeline = 
    processPipeline pipeline |> TaskSeq.toArray |> ignore

let start logger pipelines = 
    pipelines
    |> Seq.filter (fun (config, _) -> Readers.selectContinouseBasedOnly config)
    |> Seq.iter (fun (_, pipeline) -> runPipeline logger  pipeline)
