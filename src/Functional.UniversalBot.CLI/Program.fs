open System
open Configuration

let config = getConfiguration ()

printfn "Starting UniveralHiveBot processs"

let pipelines =
    config
    |> Logging.logConfigurationFound
    |> createPipelines 

pipelines
|> Scheduler.bind (Logging.renderResult "setup")
|> Scheduler.start (Logging.renderResult "setup")

pipelines
|> BackgroundTaskRunner.start (Logging.renderResult "setup")

Loop.executeLoop ()
