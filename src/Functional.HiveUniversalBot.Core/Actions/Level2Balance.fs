module Level2Balance

open PipelineResult
open Functional.ETL.Pipeline
open FsHttp
open System.Text.Json
open Types
    
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
    
let private deserialize (json: JsonElement) = 
    JsonSerializer.Deserialize<HiveResponse<TokenBalance>> json
    
let private getBalance api username = 
    let contractsUri = sprintf "%s/contracts" api
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

let private addTokenBalanceAsProperty entity tokenBalance =
    PipelineProcessData.withProperty entity tokenBalance.symbol tokenBalance.balance

let action apiUri (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    let userdata = PipelineProcessData.readPropertyAsType<UniversalHiveBotResutls, string * string * string> entity "userdata"

    match userdata with 
    | Some (username, _, _) -> 
        getBalance apiUri username
        |> Seq.fold addTokenBalanceAsProperty entity
    | _ -> 
        PipelineProcessData.withResult entity (NoUserDetails "LoadTokens")

let bind hive (urls: Urls) (parameters: Map<string, string>) = 
    action urls.hiveEngineNodeUrl