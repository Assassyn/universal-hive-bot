module ConfigurationFixture

open Xunit
open FsUnit.Xunit

[<Fact>]
let ``Can access hive engine url from settings file`` () =
    let config = Configuration.getConfiguration ()
    config.urls.hiveEngineNodeUrl |> should equal "http://engine.alamut.uk:5000"

[<Fact>]
let ``Can access hive  url from settings file`` () =
    let config = Configuration.getConfiguration ()
    config.urls.hiveNodeUrl |> should equal "https://anyx.io"

[<Fact>]
let ``Can load defined actions`` () =
    let config = Configuration.getConfiguration ()
    config.actions |> Seq.length |> should equal 1

//[<Fact>]
//let ``Create action from config`` () =
//    let config = Configuration.getConfiguration ()
//    let actions = Configuration.createPipelines config

//    actions |> Seq.length |> should equal 1
