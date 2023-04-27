module Types

type Settings =
    {
        hiveNodeUrl: string
        hiveEngineNodeUrl: string
    }

type User = 
    {
        username: string
        activeKey: string
        passiveKey: string
    }