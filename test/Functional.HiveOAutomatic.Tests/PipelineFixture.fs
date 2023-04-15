module PipelineFixture

open System
open Xunit
open FsUnit.Xunit
open Functional.ETL.Pipeline
open FSharp.Control

let private hiveNodeUrl = "http://engine.alamut.uk"
let private port = "5000"

[<Fact>]
let ``Can combine multiple using decoration pattern`` () =
    let converter1 entity =
        Entity.withProperty entity "b" 2
    let converter2 entity =
        Entity.withProperty entity "c" true

    let testEntity = 
        {
            index = 1
            properties = Map ["a", "a";]
        }

    let combinedConverter = 
        converter1 >> converter2

    let result = combinedConverter testEntity

    Entity.readProperty result "a" |> should equal "a"
    Entity.readProperty result "b" |> should equal 2
    Entity.readProperty result "c" |> should equal true

[<Fact>]
let ``Execute All readers`` () =
    let testReader () = 
        let indexes = taskSeq { yield! [1L..5L] }
        
        indexes 
        |> TaskSeq.map Entity.bind
        |> TaskSeq.map Ok 

    let testConverter entity = 
        entity

    let pipeline = Pipeline.bind testReader (Transformer.wrap testConverter)
   
    let results = process pipeline

    task {
        let! underTestResult = results |> TaskSeq.length 
        underTestResult|> should equal 5 
    } 
