module UserReaderFixture

open Xunit
open FsUnit.Xunit
open FSharp.Control

[<Fact>]
let ``Username is converted to entity`` () =
    task {
        let! result = TestingStubs.reader () |> TaskSeq.length 
        result |> should equal 1
    }

