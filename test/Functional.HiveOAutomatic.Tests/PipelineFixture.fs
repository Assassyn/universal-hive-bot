module PipelineFixture

open System
open Xunit
open FsUnit.Xunit
open Functional.ETL.Pipeline

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
        let indexes = seq { 1L .. 5L }

        indexes 
        |> Seq.map Entity.bind

    let testConverter entity = 
        entity

    let pipeline = Pipeline.bind testReader (Transformer.wrap testConverter)
   
    let results = process pipeline

    results |> Seq.length |> should equal 5 