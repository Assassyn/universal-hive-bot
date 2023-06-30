module TerracoreFixture

open System
open Xunit
open FsUnit.Xunit
open Pipeline
open FSharp.Control

let private hiveNodeUrl = "http://engine.alamut.uk"
let private port = "5000"

let extractSome (option: Option<obj>) =
    option.Value

[<Fact>]
let ``Claim Action is producing valid JSON`` () =
    task {
        let transformer = 
            [| 
                TestingStubs.mockedTerracoreBalanceAction 123M
                (TerracoreClaim.action ("*" |> AmountCalator.bind) "universal-bot") |> TestingStubs.taskDecorator
            |] |> TaskSeq.ofSeq
        let pipelineDefinition = Pipeline.bind (TestingStubs.reader) transformer
   
        processPipeline pipelineDefinition
        |> Seq.item 0
        |> fun entity -> entity.results
        |> Seq.item 0 
        |> TestingStubs.extractCustomJson 
        |> should startWith (sprintf """{"amount":"%s","tx-hash":""" "123")
    }
