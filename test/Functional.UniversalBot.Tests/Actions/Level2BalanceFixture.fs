module Level2BalanceFixture

open Xunit
open FsUnit.Xunit
open Pipeline
open FSharp.Control
open System.Threading.Tasks

let private hiveEngineNode = "http://engine.alamut.uk:5000"


let extractSome (option: Option<obj>) =
    option.Value

[<Fact>]
let ``Can read all tokens from levle 2`` () =
    task {
        let transformers = 
            [|
                (Balance.action hiveEngineNode "universal-bot") |> TestingStubs.taskDecorator
            |] |> TaskSeq.ofSeq
            
        let pipelineDefinition = Pipeline.bind TestingStubs.reader  transformers
        
        let objectUnderTest = 
            processPipeline pipelineDefinition
            |> Seq.item 0
             
        PipelineProcessData.readProperty objectUnderTest "GAMER" |> extractSome |> should equal 5M
    }
