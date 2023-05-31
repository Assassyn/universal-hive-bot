module TerracoreFixture

open System
open Xunit
open FsUnit.Xunit
open Functional.ETL.Pipeline
open FSharp.Control

let private hiveNodeUrl = "http://engine.alamut.uk"
let private port = "5000"

let extractSome (option: Option<obj>) =
    option.Value

[<Fact>]
let ``Claim Action is producing valid JSON`` () =
    let transformer = 
        (TestingStubs.mockedTerracoreBalanceAction 123M)
        >> (TerracoreClaim.action ("*" |> AmountCalator.bind))
    let pipelineDefinition = Pipeline.bind (TestingStubs.reader) transformer
   
    let results = processPipeline pipelineDefinition
    let underTestObject =
        results
        |> Seq.collect (fun x-> x.results)
        |> Seq.item 0

    underTestObject 
    |> TestingStubs.extractCustomJson 
    |> should startWith (sprintf """{"amount":"%s","tx-hash":""" "123")
