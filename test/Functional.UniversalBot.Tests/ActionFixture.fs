module ActionFixture

open Xunit
open FsUnit.Xunit
open Functional.ETL.Pipeline
open PipelineResult

let sampleAction username entity =
    entity
let moduleName = "test" 
let actionToTest = Action.bindAction moduleName sampleAction

[<Fact>]
let ``Returns no username when one not found`` () =
    task {
        let entity = PipelineProcessData.bind 1
        let! response = actionToTest entity
        response.results.Head |> should equal (UniversalHiveBotResutls.NoUserDetails moduleName)
    }
