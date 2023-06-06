open System
open Configuration

let config = getConfiguration ()

printfn "Starting UniveralHiveBot processs"

let pipelines =
    config
    |> Logging.logConfigurationFound
    |> createPipelines 

pipelines
|> Scheduler.bind Logging.writeToConsole
|> Scheduler.start Logging.writeToConsole

pipelines
|> BackgroundTaskRunner.start Logging.writeToConsole

Loop.executeLoop ()
