module SellToken 

open Core
open PipelineResult
open Types
open Functional.ETL.Pipeline

[<Literal>]
let ModuleName = "Sell"

let action logger hive hiveEngineUrl tokenSymbol (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    ()

let bind logger hive (urls: Urls) (parameters: Map<string, string>) = 
    let token = parameters.["token"]
    action logger hive urls.hiveEngineNodeUrl token
