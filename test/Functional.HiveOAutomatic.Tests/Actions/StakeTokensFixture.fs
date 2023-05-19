﻿module StakeTokensFixture

open Xunit
open FsUnit.Xunit
open Functional.ETL.Pipeline
open TestingStubs
open PipelineResult

let private hiveNodeUrl = "https://anyx.io"

let testData =
    [|
        [| ~~123M; ~~"*"; ~~"123.000" |]
        [| ~~123M; ~~"100"; ~~"100.000" |]
        [| ~~99M; ~~"100"; ~~"99.000" |]
    |]
    
[<Theory>]
[<MemberData("testData")>]
let ``Can stake tokens`` (oneUpBalance:decimal) (amountToBind: string) (result: string) =
    let reader = UserReader.bind [ ("ultimate-bot", "", "") ]
    let transformer = 
        (TestingStubs.mockedBalanceAction [| ("ONEUP", oneUpBalance) |])
        >> (StakeToken.action TestingStubs.logger "ONEUP" (AmountCalator.bind amountToBind))
    let pipelineDefinition = Pipeline.bind reader transformer
   
    let results = processPipeline pipelineDefinition
    let underTestObject =
        results
        |> Seq.collect (fun x-> x.results)
        |> Seq.item 0

    underTestObject 
    |> TestingStubs.extractCustomJson 
    |> should equal (sprintf """{"contractName":"tokens","contractAction":"stake","contractPayload":{"quantity":"%s","symbol":"ONEUP","to":"ultimate-bot"}}""" result)

[<Fact>]
let ``Check that balance is too low`` () =
    let transformer = 
        (TestingStubs.mockedDelegatedStakedBalanceAction [| ("ONEUP", 0M) |])
        >> (UndelegateStake.action TestingStubs.logger "ONEUP" "delegation-target-user" (AmountCalator.bind "100"))
    let pipelineDefinition = Pipeline.bind reader transformer

    processPipeline pipelineDefinition
    |> Seq.collect (fun x-> x.results)
    |> Seq.item 0
    |> should equal (TokenBalanceTooLow ("UndelegateStake", "ONEUP"))

[<Fact>]
let ``Check that username is required`` () =
    let transformer = 
        (TestingStubs.mockedDelegatedStakedBalanceAction [| ("ONEUP", 0M) |])
        >> (UndelegateStake.action TestingStubs.logger "ONEUP" "delegation-target-user" (AmountCalator.bind "100"))
    let pipelineDefinition = Pipeline.bind noUserReader transformer
    
    processPipeline pipelineDefinition
    |> Seq.collect (fun x-> x.results)
    |> Seq.item 0
    |> should equal (NoUserDetails ("UndelegateStake"))
