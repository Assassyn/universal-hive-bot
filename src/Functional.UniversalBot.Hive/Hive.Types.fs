module HiveTypes

type HiveResponse<'Result> =
    {
        jsonrpc: string
        id: int64
        result: 'Result
    }
