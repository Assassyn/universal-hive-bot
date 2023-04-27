module UserReader

open Functional.ETL.Pipeline
open FSharp.Control

let private createEntity index accessData =
    let (username, activeKey, postingKey) = accessData
    {
        index = int64(index)
        properties =  Map ["userdata", (username, activeKey, postingKey)]
    }


let getUserReader (userdata: (string * string * string) seq): Reader<Pipeline.HiveError> =
    fun () -> 
        userdata
        |> Seq.mapi createEntity
        |> Seq.map Result<PipelineProcessData, Pipeline.HiveError>.Ok
        |> TaskSeq.ofSeq