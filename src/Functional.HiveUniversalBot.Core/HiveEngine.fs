module CoreEngine

open System.Net.Http
open HiveAPI
open System.Text.Json
open FsHttp

type HiveResponse<'Result> =
    {
        jsonrpc: string
        id: int64
        result: 'Result seq
    }

type TokenBalance = 
    {
        _id: int64
        account: string
        symbol: string
        balance: string
        stake: string
        pendingUnstake: string
        delegationsIn: string
        delegationsOut: string
        pendingUndelegations: string
    }

type HiveEngine (hiveEngineUrl) =
    let deserialize (json: JsonElement) = 
        JsonSerializer.Deserialize<HiveResponse<TokenBalance>> json
        
    member this.getBalance username = 
        let contractsUri = sprintf "%s/contracts" hiveEngineUrl
        let response = 
            http {
                POST contractsUri
                CacheControl "no-cache"
                body
                jsonSerialize
                    {|
                        jsonrpc = "2.0"
                        id = 1
                        method = "find"
                        ``params`` = {|
                            contract = "tokens"
                            table = "balances"
                            query = {|
                                account = username
                            |}
                        |}
                    |}
            }
            |> Request.send
            |> Response.toJson
            |> deserialize
        
        response.result
