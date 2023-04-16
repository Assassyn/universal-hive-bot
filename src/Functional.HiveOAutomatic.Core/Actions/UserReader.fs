module UserReader

open Functional.ETL.Pipeline
open FSharp.Control

let private createEntity index (username: string) =
    {
        index = int64(index)
        properties =  Map ["username", username.ToLower();]
    }


let getUserReader  (usernames: string seq): Reader<Pipeline.HiveError> =
    fun () -> 
        usernames
        |> Seq.mapi createEntity
        |> Seq.map Result<PipelineProcessData, Pipeline.HiveError>.Ok
        |> TaskSeq.ofSeq