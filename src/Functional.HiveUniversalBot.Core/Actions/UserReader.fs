module UserReader

open PipelineResult
open Functional.ETL.Pipeline
open FSharp.Control

type UserData = string * string * string

let private createEntity index accessData =
    let (username, activeKey, postingKey) = accessData
    {
        index = int64(index)
        properties =  Map [
            "username", username :> obj
            "userdata", (username, activeKey, postingKey) :> obj
        ]
        results = list.Empty
    }

let bind (userdata: (string * string * string) seq) =
    fun () -> 
        userdata
        |> Seq.mapi createEntity
        |> TaskSeq.ofSeq
