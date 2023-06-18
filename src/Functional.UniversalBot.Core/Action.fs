module Action

open PipelineResult
open Functional.ETL.Pipeline
open Functional.ETL.Pipeline.PipelineProcessData

let bindAction moduleName userbasedAction (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    task {
        let userDetails: (string * string * string) option = PipelineProcessData.readPropertyAsType entity Readers.userdata

        return 
            match userDetails with 
            | Some (username, _, _) when username <> "" -> 
                userbasedAction username entity 
            | _ -> 
                NoUserDetails moduleName |> PipelineProcessData.withResult entity
    }
