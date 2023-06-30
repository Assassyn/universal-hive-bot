module FlushTokenFixture

open System
open Xunit
open FsUnit.Xunit
open Pipeline
open FSharp.Control

let private hiveNodeUrl = "http://engine.alamut.uk"
let private hiveUrl = "https://anyx.io"
let private port = "5000"

let extractSome (option: Option<obj>) =
    option.Value

[<Fact>]
let ``Can flush tokens`` () =
    task {
        let transformers = 
            [|
                (TestingStubs.mockedTerracoreBalanceAction 123M);
                (FlushTokens.action hiveUrl)
            |] |> TaskSeq.ofSeq
        let pipelineDefinition = Pipeline.bind (TestingStubs.reader) transformers
   
        processPipeline pipelineDefinition
        |> Seq.item 0
        |> fun entity -> entity.results
        |> Seq.item 0
        |> TestingStubs.extractCustomJson 
        |> should equal ""
    }
