module Level2BalanceFixture

open Xunit
open FsUnit.Xunit
open Functional.ETL.Pipeline
open FSharp.Control

let private hiveEngineNode = "http://engine.alamut.uk:5000"


let extractSome (option: Option<obj>) =
    option.Value

[<Fact>]
let ``Can read all tokens from levle 2`` () =
    let transformer = (Balance.action hiveEngineNode)
    let pipelineDefinition = Pipeline.bind TestingStubs.reader transformer
   
    let results = processPipeline pipelineDefinition
    let objectUnderTest = results |> TaskSeq.item 0
             
    PipelineProcessData.readProperty objectUnderTest "GAMER" |> extractSome |> should equal 5M
