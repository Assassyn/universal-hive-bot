namespace Functional.UniversalBot.CLI2

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Serilog
open Microsoft.Extensions.Configuration
open System.IO
open Workers

module Program =
    let private createLogger (hostingContext: HostBuilderContext) (logger: LoggerConfiguration) = 
        logger
            .ReadFrom
            .Configuration(hostingContext.Configuration)
        |> ignore

    let private addMultipleConfigFiles path (builder: IConfigurationBuilder)  = 
        Directory.GetFiles(path, "*.json")
        |> Seq.map (fun file -> builder.AddJsonFile(file, true))
        |> ignore

    let createHostBuilder args =
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(fun builder ->
                builder.AddJsonFile("configuration.json", true) |> ignore
                builder.AddUserSecrets() |> ignore
                builder |> addMultipleConfigFiles "actions")
            .ConfigureServices(fun hostContext services ->
                let configuration =
                    hostContext.Configuration
                    |> Configuration.getConfiguration
                services.AddSingleton (configuration) |> ignore
                services.AddHostedService<Workers.Worker>() |> ignore)
            .UseSerilog(createLogger)

    [<EntryPoint>]
    let main args =
        createHostBuilder(args)
            .Build()
            .Run()

        0 // exit code
