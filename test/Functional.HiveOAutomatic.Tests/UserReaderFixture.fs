module UserReaderFixture

open Xunit
open FsUnit.Xunit
open FSharp.Control

let private hiveNodeUrl = "http://engine.alamut.uk"
let private port = "5000"

[<Fact>]
let ``Username is converted to entity`` () =
    let reader = UserReader.getUserReader [ ("assassyn", "", "") ]
    task {
        let! result = reader () |> TaskSeq.length 
        result |> should equal 1
    }

