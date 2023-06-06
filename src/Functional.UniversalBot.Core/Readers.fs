module Readers

open PipelineResult
open Functional.ETL.Pipeline
open Functional.ETL.Pipeline.PipelineProcessData
open FSharp.Control
open Types


[<Literal>]
let username = "username"

[<Literal>]
let userdata = "userdata"

let private addUserDataProprties (userdataDefinition: UserActionsDefinition) (entity: PipelineProcessData<UniversalHiveBotResutls>) =
    entity
    |> addProperty username userdataDefinition.Username
    |> addProperty userdata (userdataDefinition.Username, userdataDefinition.ActiveKey, userdataDefinition.PostingKey)


let private bindOneOffReader userdata =
    fun () -> 
        [| (PipelineProcessData.bind32 0) |]
        |> TaskSeq.ofArray
        |> TaskSeq.map (addUserDataProprties userdata)

        
let private bindContinouseReader userdata =
    fun () -> 
        TaskSeq.initInfinite PipelineProcessData.bind32
        //.initInfinite PipelineProcessData.bind32
        |> TaskSeq.map (addUserDataProprties userdata)


let bindReader (userdata: UserActionsDefinition) =
    match userdata.Type with 
    | ExecutionType.Continous -> 
        let test = bindContinouseReader userdata
        test
    | ExecutionType.Scheduler -> 
        let test = bindOneOffReader userdata
        test
    

let selectSchedulerBasedOnly (userdata: UserActionsDefinition) =
    userdata.Type = ExecutionType.Scheduler

let selectContinouseBasedOnly (userdata: UserActionsDefinition) =
    userdata.Type = ExecutionType.Continous   
