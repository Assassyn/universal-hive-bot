module Level2BalanceFixture

open Xunit
open FsUnit.Xunit
open Functional.ETL.Pipeline
open Functioanl.HiveBot.HIVEConverter

let private hiveEngineNode = "http://engine.alamut.uk:5000"


let extractSome (option: Option<obj>) =
    option.Value

[<Fact>]
let ``Can read all tokens from levle 2`` () =
    let reader = UserReader.getUserReader [ ("assassyn", "", "") ]
    let transformer = (Level2Balance.action hiveEngineNode)
    let pipelineDefinition = Pipeline.bind reader transformer
   
    let results = processPipeline pipelineDefinition
    let objectUnderTest = results |> Seq.item 0
             
    PipelineProcessData.readProperty objectUnderTest "PGMM" |> extractSome |> should equal "5"
