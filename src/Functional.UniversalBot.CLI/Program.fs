open     Configuration
open PipelineResult
open Functional.ETL.Pipeline


let action () = 
    printfn "Starting UniveralHiveBot processs"
    let config = getConfiguration ()

    match config.actions with 
    | null -> 
        "no actions found"
    | _ -> 
        printfn "found %i actions to execute" (config.actions |> Seq.length)
        let pipelines = createPipelines config
        pipelines 
        |> Seq.iter (fun x -> processPipeline x |> ignore)
        "Finshed UniveralHiveBot processs"  
 
let readlines = Seq.initInfinite (fun _ -> action())
 
let run item = 
    item = "quit"
 
Seq.find run readlines |> ignore
