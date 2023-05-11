module SellTokensFixture

open Xunit
open FsUnit.Xunit
open Functional.ETL.Pipeline
open TestingStubs

let private hiveNodeUrl = "https://anyx.io"
let private hiveEngineNode = "http://engine.alamut.uk:5000"

let testData =
    [|
        [| ~~123M; ~~"*"; ~~"123" |]
        [| ~~123M; ~~"100"; ~~"100" |]
        [| ~~99M; ~~"100"; ~~"99" |]
    |]
    
[<Theory>]
[<MemberData("testData")>]
let ``Can sell tokens`` (oneUpBalance:decimal) (amountToBind: string) (result: string) =
    let reader = UserReader.bind [ ("ultimate-bot", "", "") ]
    let transformer = 
        (TestingStubs.mockedBalanceAction [| ("ONEUP", oneUpBalance) |])
        >> (SellToken.action TestingStubs.logger hiveNodeUrl hiveEngineNode "ONEUP" (AmountCalator.bind amountToBind))
    let pipelineDefinition = Pipeline.bind reader transformer
   
    let results = processPipeline pipelineDefinition
    let underTestObject =
        results
        |> Seq.collect (fun x-> x.results)
        |> Seq.item 0

    underTestObject 
    |> TestingStubs.extractCustomJson 
    |> should startWith """{"contractName":"market","contractAction":"sell","contractPayload":{"""
