open System
open Configuration

task {
    let config = getConfiguration ()

    printfn "Starting UniveralHiveBot processs"

    let pipelines =
        config
        |> Logging.logConfigurationFound
        |> Pipeline.createPipelines 

    pipelines
    |> Scheduler.bind (Logging.renderResult "setup")
    |> Scheduler.start (Logging.renderResult "setup")

    do! 
        pipelines
        |> BackgroundTaskRunner.start (Logging.renderResult "setup")
} |> Async.AwaitTask |> Async.RunSynchronously
