module StakeTokensFixture

open Xunit
open FsUnit.Xunit
open Functional.ETL.Pipeline
open Core

let private hiveNodeUrl = "https://anyx.io"
let private hiveEngineNode = "http://engine.alamut.uk:5000"

let extractSome (option: Option<obj>) =
    option.Value

[<Fact>]
let ``Can stake tokens`` () =
    let reader = UserReader.bind [ ("ultimate-bot", "", "") ]
    let hive = Hive (hiveNodeUrl)
    let transformer = 
        (Level2Balance.action hiveEngineNode)
        >> (StakeToken.action hive "ONEUP" (AmountCalator.bind "*"))
        >> (StakeToken.action hive "CENT" (AmountCalator.bind "*"))
    let pipelineDefinition = Pipeline.bind reader transformer
   
    let results = processPipeline pipelineDefinition
    let objectUnderTest = results |> Seq.item 0

    PipelineProcessData.readProperty objectUnderTest "userdata" |> extractSome |> should equal ("ultimate-bot", "", "")
    PipelineProcessData.readProperty objectUnderTest "GAMER" |> extractSome |> should equal "5"
