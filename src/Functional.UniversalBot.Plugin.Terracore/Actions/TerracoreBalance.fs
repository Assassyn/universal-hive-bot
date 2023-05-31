module TerracoreBalance

open Some
open Hive
open Decimal
open PipelineResult
open TerracoreAPI
open Hive
open Functional.ETL.Pipeline
open Functional.ETL.Pipeline.PipelineProcessData

[<Literal>]
let ModuleName = "TerracoreClaim"
[<Literal>]
let scrapHandle = "terracore_scrap" 

let action (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    let userDetails: (string * string * string) option = PipelineProcessData.readPropertyAsType entity "userdata" 

    match userDetails with 
    | Some (username, _, _) when username <> "" -> 
        let playerInfo = 
            TerracoreAPI.getPlayerInfo username

        entity 
        |> addProperty scrapHandle playerInfo.scrap
        |>= TokenBalanceLoaded username
    | _ -> 
        NoUserDetails ModuleName |> PipelineProcessData.withResult entity
        
let bind urls (parameters: Map<string, string>) = 
    action 
