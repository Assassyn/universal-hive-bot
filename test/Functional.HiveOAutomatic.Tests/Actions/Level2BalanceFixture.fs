module Level2BalanceFixture

open Xunit
open FsUnit.Xunit
open Functional.ETL.Pipeline

let private hiveEngineNode = "http://engine.alamut.uk:5000"


let extractSome (option: Option<obj>) =
    option.Value

[<Fact>]
let ``Can read all tokens from levle 2`` () =
    let reader = UserReader.bind [ ("ultimate-bot", "", "") ]
    let transformer = (Level2Balance.action TestingStubs.logger hiveEngineNode)
    let pipelineDefinition = Pipeline.bind reader transformer
   
    let results = processPipeline pipelineDefinition
    let objectUnderTest = results |> Seq.item 0
             
    PipelineProcessData.readProperty objectUnderTest "GAMER" |> extractSome |> should equal 5M
