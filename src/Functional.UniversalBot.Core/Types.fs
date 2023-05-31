module Types

open System.Collections.Generic
open Functional.ETL.Pipeline
open PipelineResult

type Urls =
    {
        hiveNodeUrl: string
        hiveEngineNodeUrl: string
    }

type ActionDefinition () = 
    let mutable name = ""
    let mutable parameters: IDictionary<string, string> = new Dictionary<string, string>()

    member this.Name 
        with get () = name
        and set (value) = name <- value
    member this.Parameters
        with get () = parameters
        and set (value) = parameters <- value 
        
type UserActionsDefinition () =
    let mutable username = ""
    let mutable activeKey = ""
    let mutable postingKey = ""
    let mutable trigger = ""
    let mutable tasks = new List<ActionDefinition>()

    member this.Username 
        with get () = username
        and set (value) = username <- value
    member this.ActiveKey 
        with get () = activeKey
        and set (value) = activeKey <- value
    member this.PostingKey 
        with get () = postingKey
        and set (value) = postingKey <- value
    member this.Tasks
        with get () = tasks
        and set (value) = tasks <- value
    member this.Trigger 
        with get () = trigger
        and set (value) = trigger <- value
        

type Configuration = 
    {
        urls: Urls
        actions: UserActionsDefinition seq
    }
    
type BindAction = Urls -> Map<string, string> -> ( PipelineProcessData<UniversalHiveBotResutls> ->  PipelineProcessData<UniversalHiveBotResutls>)
