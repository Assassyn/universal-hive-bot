module StakeToken 

open Core
open PipelineResult
open Functional.ETL.Pipeline
open FsHttp

[<Literal>]
let ModuleName = "Sell"

let action logger hive tokenSymbol (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    

let bind logger hive urls (parameters: Map<string, string>) = 
    let token = parameters.["token"]
    action logger hive token amount
