module Level2BalanceFixture

open Xunit
open FsUnit.Xunit
open Functional.ETL.Pipeline
open FSharp.Control
open System.Threading.Tasks

let private hiveEngineNode = "http://engine.alamut.uk:5000"


let extractSome (option: Option<obj>) =
    option.Value

[<Fact>]
let ``Can read all tokens from levle 2`` () =
    task {
        let transformer entity = 
            Balance.action hiveEngineNode "universal-bot" entity 
            |> Task.fromResult
        let pipelineDefinition = Pipeline.bind TestingStubs.reader  ([| transformer |] |> TaskSeq.ofArray)
   
        let! objectUnderTest = 
            processPipeline pipelineDefinition
            |> TaskSeq.item 0
             
        PipelineProcessData.readProperty objectUnderTest "GAMER" |> extractSome |> should equal 5M
    }
