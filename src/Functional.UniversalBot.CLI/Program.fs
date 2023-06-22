////open Microsoft.Extensions.Hosting

////use App.WorkerService

////let builder = Host.CreateApplicationBuilder(args)
////builder.Services.AddHostedService<Worker>();

////IHost host = builder.Build();
////host.Run();

//let config = getConfiguration ()

//printfn "Starting UniveralHiveBot processs"

//let pipelines =
//    config
//    |> Logging.logConfigurationFound
//    |> Pipeline.createPipelines 

////pipelines
////|> Scheduler.bind (Logging.renderResult "setup")
////|> Scheduler.start (Logging.renderResult "setup")

//pipelines
//|> BackgroundTaskRunner.start (Logging.renderResult "setup")
//|> Async.AwaitTask
//|> Async.RunSynchronously

//Loop.executeLoop()


//let builder =
//    new HostBuilder().ConfigureAppConfiguration((hostingContext, config) =>
//{
//// i needed the input argument for command line, you can use it or simply remove this block
//    config.AddEnvironmentVariables();

//    if (args != null)
//    {
//        config.AddCommandLine(args);
//    }

//    Shared.Configuration = config.Build();
//})
//.ConfigureServices((hostContext, services) =>
//{
//    // dependency injection
      
//    services.AddOptions();
//   // here is the core, where you inject the
//   services.AddSingleton<Daemon>();
//   services.AddSingleton<IHostedService, MyService>();
//})
//.ConfigureLogging((hostingContext, logging) => {
//   // console logging 
//    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
//    logging.AddConsole();
//});

//    await builder.RunConsoleAsync();
