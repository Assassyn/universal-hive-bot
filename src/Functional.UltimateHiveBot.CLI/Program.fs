open Configuration
open PipelineResult
open Functional.ETL.Pipeline

let printResults (processedData: PipelineProcessData<UniversalHiveBotResutls> array) = 
    processedData
    |> Seq.collect (fun x -> x.results)
    |> Seq.iter (fun result -> printfn "%O" result)

let config = getConfiguration ()
let pipelines = createPipelines config

printfn "Starting UniveralHiveBot processs"

//let test = 
pipelines 
|> Seq.map processPipeline
|> Seq.iter printResults


printfn "Finshed UniveralHiveBot processs"
