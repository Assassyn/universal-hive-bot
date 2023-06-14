module InfiniteTicker

open Types
open FSharp.Control
open Functional.ETL.Pipeline
open PipelineResult

let bind (userdata: UserActionsDefinition seq) =
    fun () -> 
        Seq.initInfinite PipelineProcessData.bind32
        |> TaskSeq.ofSeq
