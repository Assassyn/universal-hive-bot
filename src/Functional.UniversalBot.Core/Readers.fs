module Readers

open PipelineResult
open Pipeline
open PipelineProcessData
open FSharp.Control
open Types


[<Literal>]
let username = "username"

[<Literal>]
let userdata = "userdata"

[<Literal>]
let pipelineName = "pipelineName"

let private addUserDataProprties (userdataDefinition: UserActionsDefinition) (entity: PipelineProcessData<UniversalHiveBotResutls>) =
    entity
    |> addProperty username userdataDefinition.Username
    |> addProperty userdata (userdataDefinition.Username, userdataDefinition.ActiveKey, userdataDefinition.PostingKey)
    |> addProperty pipelineName userdataDefinition.Name


let bindOneOffReader userdata =
    fun () -> 
        [| (PipelineProcessData.bind32 0) |]
        |> TaskSeq.ofArray
        |> TaskSeq.map (addUserDataProprties userdata)

        
//let private bindContinouseReader userdata =
//    fun () -> 
//        TaskSeq.initInfinite PipelineProcessData.bind32
//        |> TaskSeq.map (addUserDataProprties userdata)


//let bindReader (userdata: UserActionsDefinition) =
//    bindOneOffReader userdata
//    //match userdata.Type with 
//    //| ExecutionType.Continous -> 
//    //    bindContinouseReader userdata
//    //| _ -> 
//    //    bindOneOffReader userdata
    

//let selectSchedulerBasedOnly (userdata: UserActionsDefinition) =
//    userdata.Type = ExecutionType.Scheduler

//let selectContinouseBasedOnly (userdata: UserActionsDefinition) =
//    userdata.Type = ExecutionType.Continous   
