module CLITimerFixture

open System
open Xunit
open FsUnit.Xunit
open FSharp.Control
open TestingStubs
open Pipeline

let testData =
    [|
        [| ~~"2023-06-25 12:00:00"; ~~"2023-06-25 12:00:00"; ~~"Scheduler"; ~~"0 */1 * * *"; ~~false |]
        [| ~~"2023-06-25 11:59:59"; ~~"2023-06-25 11:49:59"; ~~"Scheduler"; ~~"0 */1 * * *"; ~~false |]
        [| ~~"2023-06-25 11:59:59"; ~~"2023-06-25 11:59:07"; ~~"Scheduler"; ~~"0 */1 * * *"; ~~false |]
        [| ~~"2023-06-25 11:59:59"; ~~"2023-06-25 12:00:07"; ~~"Scheduler"; ~~"0 */1 * * *"; ~~true |]
    |]

[<Theory>]
[<MemberData("testData")>]
let ``Test task scheduling`` previousActionStartedAt timeNow actionType trigger expectedResult =
    let settings = Map [ 
        "type", actionType 
        "trigger", trigger ]
    let pipeline = Pipeline.bindCustom "test" settings TestingStubs.reader (Seq.empty |> TaskSeq.ofSeq)
    let timeProvider () = timeNow |> DateTime.Parse
    Workers.canExecute timeProvider (previousActionStartedAt |> DateTime.Parse) pipeline
    |> should equal expectedResult
