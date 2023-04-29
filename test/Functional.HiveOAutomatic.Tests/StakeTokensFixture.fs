module StakeTokensFixture

open Xunit
open FsUnit.Xunit
open Functional.ETL.Pipeline
open Functioanl.HiveBot.HIVEConverter
open Core

let private hiveNodeUrl = "https://anyx.io"
let private hiveEngineNode = "http://engine.alamut.uk:5000"

let extractSome (option: Option<obj>) =
    option.Value

[<Fact>]
let ``Can stake tokens`` () =
    let reader = UserReader.getUserReader [ ("assassyn", "", "") ]
    let hive = Hive (hiveNodeUrl)
    let transformer = 
        (Level2Balance.action hiveEngineNode)
        >> (StakeToken.action hive ["ONEUP"; "CENT"; "PGM"])
    let pipelineDefinition = Pipeline.bind reader transformer
   
    let results = processPipeline pipelineDefinition
    let objectUnderTest = results |> Seq.item 0

    PipelineProcessData.readProperty objectUnderTest "userdata" |> extractSome |> should equal ("assassyn", "", "")
    PipelineProcessData.readProperty objectUnderTest "PGMM" |> extractSome |> should equal "5"
