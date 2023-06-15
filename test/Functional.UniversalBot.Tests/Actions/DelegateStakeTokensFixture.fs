module DelegateStakeTokensFixture

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
let ``Can delegate stake tokens`` oneUpBalance amountToBind result =
    task {
        let transformer = 
            (TestingStubs.mockedStakedBalanceAction [| ("ONEUP", oneUpBalance) |])
            >> (DelegateStake.action "ONEUP" "delegation-target-user" (AmountCalator.bind amountToBind) "universal-bot")
        let pipelineDefinition = Pipeline.bind reader transformer
   
        let results = processPipeline pipelineDefinition
        let! underTestObject =
            results
            |> TaskSeq.collect (fun x-> x.results |> TaskSeq.ofList)
            |> TaskSeq.item 0

        underTestObject 
        |> TestingStubs.extractCustomJson 

        |> should equal (sprintf """{"contractName":"tokens","contractAction":"delegate","contractPayload":{"quantity":"%s","symbol":"ONEUP","to":"delegation-target-user"}}""" result)
    }

[<Fact>]
let ``Check that balance is too low`` () =
    let transformer = 
        (TestingStubs.mockedDelegatedStakedBalanceAction [| ("ONEUP", 0M) |])
        >> (DelegateStake.action "ONEUP" "delegation-target-user" (AmountCalator.bind "100") "universal-bot")
    let pipelineDefinition = Pipeline.bind reader transformer

    processPipeline pipelineDefinition
    |> TaskSeq.collect (fun x-> x.results |> TaskSeq.ofList)
    |> TaskSeq.item 0
    |> Async.AwaitTask
    |> Async.RunSynchronously
    |> should equal (TokenBalanceTooLow ("DelegateStake", "universal-bot", "ONEUP"))
