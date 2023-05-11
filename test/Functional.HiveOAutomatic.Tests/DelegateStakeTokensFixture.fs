module DelegateStakeTokensFixture

open Xunit
open FsUnit.Xunit
open Functional.ETL.Pipeline
open TestingStubs

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
    let reader = UserReader.bind [ ("ultimate-bot", "", "") ]
    let transformer = 
        (TestingStubs.mockedStakedBalanceAction [| ("ONEUP", oneUpBalance) |])
        >> (DelegateStake.action TestingStubs.logger "ONEUP" "delegation-target-user" (AmountCalator.bind amountToBind))
    let pipelineDefinition = Pipeline.bind reader transformer
   
    let results = processPipeline pipelineDefinition
    let underTestObject =
        results
        |> Seq.collect (fun x-> x.results)
        |> Seq.item 0

    underTestObject 
    |> TestingStubs.extractCustomJson 

    |> should equal (sprintf """{"contractName":"tokens","contractAction":"delegate","contractPayload":{"quantity":"%s","symbol":"ONEUP","to":"delegation-target-user"}}""" result)
