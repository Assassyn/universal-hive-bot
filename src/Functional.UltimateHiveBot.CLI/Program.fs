open Configuration
open Functional.ETL.Pipeline

let printResults processedData = 
    printf "processed"

let config = getConfiguration ()
let pipelines = createPipelines config

printfn "Starting UniveralHiveBot processs"

pipelines 
|> Seq.map processPipeline
|> Seq.iter printResults


printfn "Finshed UniveralHiveBot processs"
