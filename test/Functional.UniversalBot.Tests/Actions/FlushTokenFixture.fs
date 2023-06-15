﻿module FlushTokenFixture

open System
open Xunit
open FsUnit.Xunit
open Functional.ETL.Pipeline
open FSharp.Control

let private hiveNodeUrl = "http://engine.alamut.uk"
let private hiveUrl = "https://anyx.io"
let private port = "5000"

let extractSome (option: Option<obj>) =
    option.Value

[<Fact>]
let ``Can flush tokens`` () =
    task {
        let transformer = 
            (TestingStubs.mockedTerracoreBalanceAction 123M)
            >> (FlushTokens.action hiveUrl)
        let pipelineDefinition = Pipeline.bind (TestingStubs.reader) transformer
   
        let results = processPipeline pipelineDefinition
        let! underTestObject =
            results
            |> TaskSeq.collect (fun x-> x.results |> TaskSeq.ofList)
            |> TaskSeq.item 0

        underTestObject 
        |> TestingStubs.extractCustomJson 
        |> should equal ""
    }