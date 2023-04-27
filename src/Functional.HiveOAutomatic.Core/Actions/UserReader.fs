module UserReader

open Functional.ETL.Pipeline
open FSharp.Control

let private createEntity index accessData =
    let (username, activeKey, postingKey) = accessData
    {
        index = int64(index)
        properties =  Map ["userdata", (username, activeKey, postingKey)]
        results = list.Empty
    }

let getUserReader (userdata: (string * string * string) seq): Reader<Pipeline.UniversalHiveBotResutls> =
    fun () -> 
        userdata
        |> Seq.mapi createEntity
        |> TaskSeq.ofSeq