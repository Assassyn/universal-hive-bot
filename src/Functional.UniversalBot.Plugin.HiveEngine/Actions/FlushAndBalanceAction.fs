module FlushAndBalanceAction

open System
open System.Threading
open PipelineResult
open Pipeline
open Types
open System.Threading.Tasks

[<Literal>]
let private ModuleName = "FlushAndBalance"

let private waitTime = TimeSpan.FromSeconds (3)

let action hiveUrl hiveEngineUrl (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    task {
        let! resultEntity = FlushTokens.action hiveUrl entity
        //|> (fun entity ->
        //    Thread.Sleep (waitTime)
        //    entity)
        //|> Balance.action hiveEngineUrl

        do! Task.Delay(TimeSpan.FromSeconds 3)

        return Balance.action hiveEngineUrl "" resultEntity
    }

let bind urls (parameters: Map<string, string>) = 
    action urls.hiveNodeUrl urls.hiveEngineNodeUrl
