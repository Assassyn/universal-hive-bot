module Types

type Urls =
    {
        hiveNodeUrl: string
        hiveEngineNodeUrl: string
    }

type ActionDefinition = 
    {
        name: string
        parameters: Map<string, string>
    }
type UserActionsDefinition =
    {
        username: string
    }

type Configuration = 
    {
        urls: Urls
        actions: ActionDefinition seq
    }

