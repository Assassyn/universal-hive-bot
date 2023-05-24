module SellTokensFixture

open Xunit
open FsUnit.Xunit
open Functional.ETL.Pipeline
open TestingStubs
open PipelineResult

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
    let transformer = 
        (TestingStubs.mockedBalanceAction [| ("ONEUP", oneUpBalance) |])
        >> (SellToken.action hiveNodeUrl hiveEngineNode "ONEUP" (AmountCalator.bind amountToBind))
    let pipelineDefinition = Pipeline.bind TestingStubs.reader transformer
   
    let results = processPipeline pipelineDefinition
    let underTestObject =
        results
        |> Seq.collect (fun x-> x.results)
        |> Seq.item 0

    underTestObject 
    |> TestingStubs.extractCustomJson 
    |> should startWith """{"contractName":"market","contractAction":"sell","contractPayload":{"""

[<Fact>]
let ``Check that balance is too low`` () =
    let transformer = 
        (TestingStubs.mockedDelegatedStakedBalanceAction [| ("ONEUP", 0M) |])
        >> (UndelegateStake.action "ONEUP" "delegation-target-user" (AmountCalator.bind "100"))
    let pipelineDefinition = Pipeline.bind TestingStubs.reader transformer

    processPipeline pipelineDefinition
    |> Seq.collect (fun x-> x.results)
    |> Seq.item 0
    |> should equal (TokenBalanceTooLow ("UndelegateStake", "ultimate-bot", "ONEUP"))

[<Fact>]
let ``Check that username is required`` () =
    let transformer = 
        (TestingStubs.mockedDelegatedStakedBalanceAction [| ("ONEUP", 0M) |])
        >> (UndelegateStake.action "ONEUP" "delegation-target-user" (AmountCalator.bind "100"))
    let pipelineDefinition = Pipeline.bind noUserReader transformer
    
    processPipeline pipelineDefinition
    |> Seq.collect (fun x-> x.results)
    |> Seq.item 0
    |> should equal (NoUserDetails ("UndelegateStake"))
