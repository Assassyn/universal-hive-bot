open System
open System.Threading
open Configuration
open PipelineResult
open Functional.ETL.Pipeline

let config = getConfiguration ()

printfn "Starting UniveralHiveBot processs"

config
|> Scheduler.bind 
|> Scheduler.start

Loop.executeLoop ()
