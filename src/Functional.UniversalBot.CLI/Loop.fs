module Loop

open System
open System.Threading
open Configuration
open PipelineResult
open Functional.ETL.Pipeline

let action () = 
    printfn "Press q to quit"
    let key = Console.ReadKey()
    key.Key.ToString()
 
let readlines = Seq.initInfinite (fun _ -> action())
 
let run (item: string) = 
    "q".Equals(item, StringComparison.OrdinalIgnoreCase)

let executeLoop () =
    Seq.find run readlines |> ignore
