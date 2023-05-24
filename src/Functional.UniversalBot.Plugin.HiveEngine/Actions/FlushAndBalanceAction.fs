module FlushAndBalanceAction

open System
open System.Threading
open PipelineResult
open Functional.ETL.Pipeline
open Types

[<Literal>]
let private ModuleName = "FlushAndBalance"

let private waitTime = TimeSpan.FromSeconds (3)

let action hiveUrl hiveEngineUrl (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    FlushTokens.action hiveUrl entity
    |> (fun entity ->
        Thread.Sleep (waitTime)
        entity)
    |> Balance.action hiveEngineUrl

let bind urls (parameters: Map<string, string>) = 
    action urls.hiveNodeUrl urls.hiveEngineNodeUrl
