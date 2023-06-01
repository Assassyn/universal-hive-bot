module Action

open PipelineResult
open Functional.ETL.Pipeline
open Functional.ETL.Pipeline.PipelineProcessData

let bindAction moduleName userbasedAction (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    let userDetails: (string * string * string) option = PipelineProcessData.readPropertyAsType entity "userdata" 

    match userDetails with 
    | Some (username, _, _) when username <> "" -> 
        userbasedAction username entity 
    | _ -> 
        NoUserDetails moduleName |> PipelineProcessData.withResult entity
