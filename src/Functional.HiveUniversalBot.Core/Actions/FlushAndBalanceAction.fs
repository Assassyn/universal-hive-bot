module FlushAndBalanceAction

open System
open System.Threading
open PipelineResult
open Functional.ETL.Pipeline
open Types

[<Literal>]
let private ModuleName = "FlushAndBalance"

let private waitTime = TimeSpan.FromSeconds (3)

let action logger hiveUrl (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    let entity = FlushTokens.action logger hiveUrl entity
    Thread.Sleep (waitTime)
    Level2Balance.action logger hiveUrl (PipelineProcessData.bind 0)

let bind logger urls (parameters: Map<string, string>) = 
    action (logger ModuleName) urls.hiveNodeUrl
