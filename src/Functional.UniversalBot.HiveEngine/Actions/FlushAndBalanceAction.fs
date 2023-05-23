module FlushAndBalanceAction

open System
open System.Threading
open PipelineResult
open Functional.ETL.Pipeline
open Types

[<Literal>]
let private ModuleName = "FlushAndBalance"

let private waitTime = TimeSpan.FromSeconds (3)

let action logger hiveUrl hiveEngineUrl (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    FlushTokens.action logger hiveUrl entity
    |> (fun entity ->
        Thread.Sleep (waitTime)
        entity)
    |> Balance.action logger hiveEngineUrl

let bind logger urls (parameters: Map<string, string>) = 
    action (logger ModuleName) urls.hiveNodeUrl urls.hiveEngineNodeUrl
