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

let action username (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    let playerInfo = 
        TerracoreAPI.getPlayerInfo username

    entity 
    |> addProperty scrapHandle playerInfo.scrap
    |>= TokenBalanceLoaded username
        
let bind urls (parameters: Map<string, string>) = 
    Action.bindAction ModuleName action 
