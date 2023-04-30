module Types

open System.Collections.Generic

type Urls =
    {
        hiveNodeUrl: string
        hiveEngineNodeUrl: string
    }

type ActionDefinition () = 
    let mutable name = ""
    let mutable parameters = new Dictionary<string, string>()

    member this.Name 
        with get () = name
        and set (value) = name <- value
    member this.Parameters 
        with get () = parameters
        and set (value) = parameters <- value
    //{
    //    name: string
    //    parameters: Dictionary<string, string>
    //}
        
type UserActionsDefinition () =
    let mutable username = ""
    let mutable activeKey = ""
    let mutable postingKey = ""
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
    //{
    //    username: string
    //    activeKey: string
    //    postingKey: string
    //    tasks: List<ActionDefinition>
    //}

type Configuration = 
    {
        urls: Urls
        actions: UserActionsDefinition seq
    }

