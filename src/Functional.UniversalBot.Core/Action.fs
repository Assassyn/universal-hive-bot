module Action

open PipelineResult
open Functional.ETL.Pipeline
open Functional.ETL.Pipeline.PipelineProcessData
open System.Threading.Tasks

let bindAction moduleName userbasedAction (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    let userDetails: (string * string * string) option = PipelineProcessData.readPropertyAsType entity Readers.userdata

    match userDetails with 
    | Some (username, _, _) when username <> "" -> 
        userbasedAction username entity |> Task.FromResult
    | _ -> 
        NoUserDetails moduleName |> PipelineProcessData.withResult entity |> Task.FromResult

let bindAsyncAction moduleName userbasedAction (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    task {
        let userDetails: (string * string * string) option = PipelineProcessData.readPropertyAsType entity Readers.userdata

        match userDetails with 
        | Some (username, _, _) when username <> "" -> 
            let! result = userbasedAction username entity 
            return result
        | _ -> 
            return NoUserDetails moduleName |> PipelineProcessData.withResult entity
    }
