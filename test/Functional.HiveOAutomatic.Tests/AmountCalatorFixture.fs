module AmountCalatorFixture

open Xunit
open FsUnit.Xunit
open FSharp.Control

let inline private (~~) x = x :> obj
let amountToParse =
    [|
        [| ~~"*"; ~~10M; ~~10M |]
        [| ~~"*"; ~~100M; ~~100M |]
        [| ~~"* - 5"; ~~10M; ~~5M |]
        [| ~~"* - 5"; ~~4M; ~~0M |]
        [| ~~"* - 5"; ~~5M; ~~0M |]
        [| ~~"150"; ~~1000M; ~~150M |]
        [| ~~"150"; ~~150M; ~~150M |]
        [| ~~"150"; ~~10M; ~~10M |]
    |]

[<Theory>]
[<MemberData("amountToParse")>]
let ``Can accept any amount`` (valueToParseFormat: string) (givenValue: decimal) (result: decimal) =
    let calcualtor = AmountCalator.bind valueToParseFormat

    calcualtor givenValue |> should equal result
