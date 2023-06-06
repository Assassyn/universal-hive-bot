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
        
[<Literal>]
let private defaultTrigger = "0 0 */1 * * *"

type ExecutionType =
    | Scheduler = 0
    | Continous = 1 

type UserActionsDefinition () =
    let mutable username = ""
    let mutable name = ""
    let mutable activeKey = ""
    let mutable postingKey = ""
    let mutable trigger = defaultTrigger
    let mutable executionType = ExecutionType.Scheduler
    let mutable tasks = new List<ActionDefinition>()

    member this.Username 
        with get () = username
        and set (value) = username <- value
    member this.Name 
        with get () = 
            if System.String.IsNullOrEmpty(name)  
            then
                username
            else 
                name
        and set (value) = name <- value
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
    member this.Type 
        with get () = executionType
        and set (value) = executionType <- value
        

type Configuration = 
    {
        urls: Urls
        actions: UserActionsDefinition seq
    }
    
type BindAction = Urls -> Map<string, string> -> ( PipelineProcessData<UniversalHiveBotResutls> ->  PipelineProcessData<UniversalHiveBotResutls>)
