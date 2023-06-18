﻿module StakeTokensFixture

open Xunit
open FsUnit.Xunit
open Functional.ETL.Pipeline
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
let ``Can stake tokens`` (oneUpBalance:decimal) (amountToBind: string) (result: string) =
    task {
        let transformer = 
            [|
                (TestingStubs.mockedBalanceAction [| ("ONEUP", oneUpBalance) |])
                (StakeToken.action "ONEUP" (AmountCalator.bind amountToBind) "universal-bot") |> TestingStubs.taskDecorator
            |] |> TaskSeq.ofSeq
        let pipelineDefinition = Pipeline.bind reader transformer
   
        let results = processPipeline pipelineDefinition
        let! underTestObject =
            results
            |> TaskSeq.collect (fun x-> x.results |> TaskSeq.ofList)
            |> TaskSeq.item 0

        underTestObject 
        |> TestingStubs.extractCustomJson 
        |> should equal (sprintf """{"contractName":"tokens","contractAction":"stake","contractPayload":{"quantity":"%s","symbol":"ONEUP","to":"universal-bot"}}""" result)
    }

[<Fact>]
let ``Check that balance is too low`` () =
    let transformer = 
        [|
            (TestingStubs.mockedDelegatedStakedBalanceAction [| ("ONEUP", 0M) |])
            (UndelegateStake.action "ONEUP" "delegation-target-user" (AmountCalator.bind "100") "universal-bot") |> TestingStubs.taskDecorator
         |] |> TaskSeq.ofSeq
    let pipelineDefinition = Pipeline.bind reader transformer

    processPipeline pipelineDefinition
    |> TaskSeq.collect (fun x-> x.results |> TaskSeq.ofList)
    |> TaskSeq.item 0
    |> Async.AwaitTask
    |> Async.RunSynchronously
    |> should equal (TokenBalanceTooLow ("UndelegateStake", "universal-bot", "ONEUP"))
