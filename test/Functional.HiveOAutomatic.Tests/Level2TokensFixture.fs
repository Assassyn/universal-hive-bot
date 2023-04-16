module Level2TokensFixture

open Xunit
open FsUnit.Xunit
open FSharp.Control
open Xunit
open FsUnit.Xunit
open Functional.ETL.Pipeline
open FSharp.Control
open Functioanl.HiveBot
open Functioanl.HiveBot.HIVEConverter

let private hiveNodeUrl = "http://engine.alamut.uk"
let private port = "5000"

[<Fact>]
let ``Username is converted to entity`` () =
    let reader = UserReader.getUserReader [ "assassyn" ]
    let transformer = Transformer.wrap LoadLevel2Tokens.LoadLevel2Tokens
    let pipelineDefinition = Pipeline.bind reader transformer
   
    let results = processPipeline pipelineDefinition
    ()
