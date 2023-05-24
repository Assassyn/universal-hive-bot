module UserReader

open PipelineResult
open Functional.ETL.Pipeline
open FSharp.Control
open Types

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

let bind (userdata: UserActionsDefinition seq) =
    fun () -> 
        userdata
        |> Seq.map (fun config -> (config.Username, config.ActiveKey, config.PostingKey))
        |> Seq.mapi createEntity
        |> TaskSeq.ofSeq
