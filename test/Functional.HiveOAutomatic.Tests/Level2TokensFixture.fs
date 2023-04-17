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

let extractSome (option: Option<obj>) =
    option.Value

[<Fact>]
let ``Can read all tokens from levle 2`` () =
    let reader = UserReader.getUserReader [ "assassyn" ]
    let transformer = Transformer.wrap LoadLevel2Tokens.LoadLevel2Tokens
    let pipelineDefinition = Pipeline.bind reader transformer
   
    let results = processPipeline pipelineDefinition
    let objectUnderTest = 
        results 
        |> Seq.item 0
        |> function 
            | Ok i -> i
            | _ -> PipelineProcessData.bind 0

    PipelineProcessData.readProperty objectUnderTest "username" |> extractSome |> should equal "assassyn"
    PipelineProcessData.readProperty objectUnderTest "PGMM" |> extractSome |> should equal 5
