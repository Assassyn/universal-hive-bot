module StakeTokensFixture

open Xunit
open FsUnit.Xunit
open FSharp.Control
open Xunit
open FsUnit.Xunit
open Functional.ETL.Pipeline
open FSharp.Control
open Functioanl.HiveBot
open Functioanl.HiveBot.HIVEConverter

let private hiveNodeUrl = "https://anyx.io"
let private hiveEngineNode = "http://engine.alamut.uk:5000"
let private port = "5000"


let extractSome (option: Option<obj>) =
    option.Value

[<Fact>]
let ``Can stake tokens`` () =
    let reader = UserReader.getUserReader [ ("assassyn", "", "") ]
    let hive = Hive.Hive (hiveNodeUrl)
    let transformer = 
        (LoadLevel2Tokens.action hiveEngineNode)
        >> (StakeToken.action hive ["ONEUP"; "CENT"; "PGM"])
    let pipelineDefinition = Pipeline.bind reader transformer
   
    let results = processPipeline pipelineDefinition
    let objectUnderTest = results |> Seq.item 0

    PipelineProcessData.readProperty objectUnderTest "userdata" |> extractSome |> should equal ("assassyn", "", "")
    PipelineProcessData.readProperty objectUnderTest "PGMM" |> extractSome |> should equal "5"
