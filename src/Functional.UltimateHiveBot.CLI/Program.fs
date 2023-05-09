open Configuration
open PipelineResult
open Functional.ETL.Pipeline

let printResults (processedData: PipelineProcessData<UniversalHiveBotResutls> array) = 
    processedData
    |> Seq.collect (fun x -> x.results)
    |> Seq.iter (fun result -> printfn "%O" result)

let getlastResult (entity:PipelineProcessData<UniversalHiveBotResutls>) =
    match entity.results.IsEmpty with 
    | false -> Some entity.results.[^0]
    | _ -> None

let logger moduleName user token =
    printfn "%s: %s: %s" moduleName user token

let config = getConfiguration ()
let pipelines = createPipelines config logger 

printfn "Starting UniveralHiveBot processs"
pipelines 
|> Seq.iter (fun x -> processPipeline x |> ignore)
//|> Seq.iter printResults
printfn "Finshed UniveralHiveBot processs"
