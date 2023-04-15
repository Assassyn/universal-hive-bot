module UserReader

open Functional.ETL.Pipeline
open FSharp.Control

let private createEntity index (username: string) =
    {
        index = int64(index)
        properties =  Map ["username", username.ToLower();]
    }


let getUserReader (usernames: string seq) =
    usernames
    |> Seq.mapi createEntity
    |> TaskSeq.ofSeq

