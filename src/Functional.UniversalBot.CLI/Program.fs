open     Configuration
open PipelineResult
open Functional.ETL.Pipeline

printfn "Starting UniveralHiveBot processs"

let logMessage message = 
    printfn "%s"message

let config = getConfiguration ()

let logging (entity: PipelineProcessData<PipelineResult.UniversalHiveBotResutls>) =
    match entity.results |> Seq.length with 
    | 0 -> 
        entity
    | _ -> 
        let head = entity.results.Head
        printfn "%O" head
        entity

type LogginSandwitch = 
    {
        beforeLogger: PipelineProcessData<PipelineResult.UniversalHiveBotResutls> -> PipelineProcessData<PipelineResult.UniversalHiveBotResutls>
        afterLogger: PipelineProcessData<PipelineResult.UniversalHiveBotResutls> -> PipelineProcessData<PipelineResult.UniversalHiveBotResutls>
    }
    

match config.actions with 
| null -> 
    printfn "no actions found"
| _ -> 
    printfn "found %i actions to execute" (config.actions |> Seq.length)
    let pipelines = createPipelines config
    pipelines 
    |> Seq.iter (fun x -> processPipeline x |> ignore)

printfn "Finshed UniveralHiveBot processs"

//let action = fun _ ->
//    Console.Write "\nEnter input: "
//    Console.ReadLine()
 
//let readlines = Seq.init_infinite (fun _ -> action())
 
//let run item = if item = "quit"
//                then Some(item)
//                else
//                    parse item
//                    None
 
//Seq.first run readlines |> ignore
//Console.WriteLine "Thanks! Come Again"
