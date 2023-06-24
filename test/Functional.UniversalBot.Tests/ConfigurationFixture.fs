module ConfigurationFixture

open Xunit
open FsUnit.Xunit
open Microsoft.Extensions.Configuration

let configuration = 
    let items = 
        [ 
            "urls:hiveNodeUrl", "https://anyx.io"
            "urls:hiveEngineNodeUrl", "http://engine.alamut.uk:5000"
            "actions:[0]:username", "universal-bot"
            "actions:[0]:activeKey", ""
            "actions:[0]:postingKey", ""
        ]
        |> Map.ofList
    (new ConfigurationBuilder()).AddInMemoryCollection(items).Build()

[<Fact>]
let ``Can access hive engine url from settings file`` () =
    let config = Configuration.getConfiguration configuration
    config.urls.hiveEngineNodeUrl |> should equal "http://engine.alamut.uk:5000"

[<Fact>]
let ``Can access hive  url from settings file`` () =
    let config = Configuration.getConfiguration configuration
    config.urls.hiveNodeUrl |> should equal "https://anyx.io"

[<Fact>]
let ``Can load defined actions`` () =
    let config = Configuration.getConfiguration configuration
    config.actions |> Seq.length |> should greaterThanOrEqualTo 1
