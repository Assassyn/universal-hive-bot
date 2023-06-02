module HiveMessagesPipelineFixture

open System
open Xunit
open FsUnit.Xunit
open Functional.ETL.Pipeline
open FSharp.Control

let private hiveNodeUrl = "http://engine.alamut.uk"
let private port = "5000"
let private hiveUrl = "https://anyx.io"

let extractSome (option: Option<obj>) =
    option.Value

[<Fact>]
let ``Can list all messages on the blockchain`` () =
    let headBlockNumber = CondenserApi.getHeadBlock hiveUrl

    headBlockNumber |> should greaterThan 0L
