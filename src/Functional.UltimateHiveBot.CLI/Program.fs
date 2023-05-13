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

printfn "Starting UniveralHiveBot processs"

let config = getConfiguration ()

match config.actions with 
| null -> 
    printfn "no actions found"
| _ -> 
    printfn "found %i actions to execute" (config.actions |> Seq.length)
    let pipelines = createPipelines config logger 
    pipelines 
    |> Seq.iter (fun x -> processPipeline x |> ignore)

printfn "Finshed UniveralHiveBot processs"
