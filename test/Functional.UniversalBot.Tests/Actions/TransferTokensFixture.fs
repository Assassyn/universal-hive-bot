module TransferTokensFixture

open Xunit
open FsUnit.Xunit
open Pipeline
open TestingStubs
open PipelineResult
open FSharp.Control

let private hiveNodeUrl = "https://anyx.io"

let testData =
    [|
        [| ~~123M; ~~"*"; ~~"123" |]
        [| ~~123M; ~~"100"; ~~"100" |]
        [| ~~99M; ~~"100"; ~~"99" |]
    |]
    
[<Theory>]
[<MemberData("testData")>]
let ``Can transfer tokens`` (oneUpBalance:decimal) (amountToBind: string) (result: string) =
    task {
        let transformer = 
            [|
                (TestingStubs.mockedBalanceAction [| ("ONEUP", oneUpBalance) |])
                (TransferToken.action "ONEUP" "universal-bot" (AmountCalator.bind amountToBind) "" "universal-bot") |> TestingStubs.taskDecorator
            |] |> TaskSeq.ofSeq 
        let pipelineDefinition = Pipeline.bind reader transformer
   
        processPipeline pipelineDefinition
        |> Seq.item 0
        |> fun entity -> entity.results
        |> Seq.item 0
        |> TestingStubs.extractCustomJson 
        |> should equal (sprintf """{"contractName":"tokens","contractAction":"transfer","contractPayload":{"memo":"","quantity":"%s","symbol":"ONEUP","to":"universal-bot"}}""" result)
    }

[<Fact>]
let ``Check that balance is too low`` () =
    task {
        let transformer = 
            [|
                (TestingStubs.mockedDelegatedStakedBalanceAction [| ("ONEUP", 0M) |])
                (TransferToken.action "ONEUP" "delegation-target-user" (AmountCalator.bind "100") "" "universal-bot") |> TestingStubs.taskDecorator
            |] |> TaskSeq.ofSeq
        let pipelineDefinition = Pipeline.bind reader transformer
    
        processPipeline pipelineDefinition
        |> Seq.item 0
        |> fun entity -> entity.results
        |> Seq.item 0
        |> should equal (TokenBalanceTooLow ("Transfer", "universal-bot", "ONEUP"))
    }
