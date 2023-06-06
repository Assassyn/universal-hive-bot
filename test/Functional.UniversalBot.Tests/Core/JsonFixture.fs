module JsonFixture

open Xunit
open System.Text.Json
open FsUnit.Xunit

[<Fact>]
let ``Can deserialzie object `` () =
    let json = """{"name":"test"}"""
    let jsonDocument = JsonDocument.Parse json

    jsonDocument.RootElement 
    |> Json.deserialize<{|name: string|}>
    |> should equal {| name = "test" |}

