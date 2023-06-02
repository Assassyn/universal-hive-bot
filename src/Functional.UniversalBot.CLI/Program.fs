open System
open Configuration

let config = getConfiguration ()

printfn "Starting UniveralHiveBot processs"

config
|> Logging.logConfigurationFound
|> Scheduler.bind Logging.writeToConsole
|> Scheduler.start Logging.writeToConsole

Loop.executeLoop ()
