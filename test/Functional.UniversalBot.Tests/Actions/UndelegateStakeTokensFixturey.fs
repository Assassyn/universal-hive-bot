module UndelegateStakeTokensFixturey

open Xunit
open FsUnit.Xunit
open Functional.ETL.Pipeline
open TestingStubs
open PipelineResult

let private hiveNodeUrl = "https://anyx.io"

let testData =
    [|
        [| ~~123M; ~~"*"; ~~"123.00000" |]
        [| ~~123M; ~~"100"; ~~"100.00000" |]
        [| ~~99M; ~~"100"; ~~"99.00000" |]
    |]
    
[<Theory>]
[<MemberData("testData")>]
let ``Can delegate stake tokens`` oneUpBalance amountToBind result =
    let transformer = 
        (TestingStubs.mockedDelegatedStakedBalanceAction [| ("ONEUP", oneUpBalance) |])
        >> (UndelegateStake.action "ONEUP" "delegation-target-user" (AmountCalator.bind amountToBind) "universal-bot")
    let pipelineDefinition = Pipeline.bind reader transformer
   
    processPipeline pipelineDefinition
    |> Seq.collect (fun x-> x.results)
    |> Seq.item 0
    |> TestingStubs.extractCustomJson 
    |> should equal (sprintf """{"contractName":"tokens","contractAction":"undelegate","contractPayload":{"from":"delegation-target-user","quantity":"%s","symbol":"ONEUP"}}""" result)

[<Fact>]
let ``Check that balance is too low`` () =
    let transformer = 
        (TestingStubs.mockedDelegatedStakedBalanceAction [| ("ONEUP", 0M) |])
        >> (UndelegateStake.action "ONEUP" "delegation-target-user" (AmountCalator.bind "100") "universal-bot")
    let pipelineDefinition = Pipeline.bind reader transformer
    
    processPipeline pipelineDefinition
    |> Seq.collect (fun x-> x.results)
    |> Seq.item 0
    |> should equal (TokenBalanceTooLow ("UndelegateStake", "universal-bot", "ONEUP"))
