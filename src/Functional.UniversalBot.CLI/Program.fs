open System
open System.Threading
open Configuration
open PipelineResult
open Functional.ETL.Pipeline

let config = getConfiguration ()

printfn "Starting UniveralHiveBot processs"

config
|> Logging.logConfigurationFound
|> Scheduler.bind Logging.writeToConsole
|> Scheduler.start Logging.writeToConsole

Loop.executeLoop ()
