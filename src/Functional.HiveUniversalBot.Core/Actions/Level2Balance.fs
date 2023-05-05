module Level2Balance

open PipelineResult
open Functional.ETL.Pipeline
open FsHttp
open System.Text.Json
open Types
open System
    
[<Literal>]
let private ModuleName = "Balance"

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

let stringAsDecimal (input: string) =
    let mutable number = 0M
    if Decimal.TryParse  (input, &number)
    then 
        number
    else 
        0M
    
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

let private addTokenBalanceAsProperty entity tokenInfo =
    let tokenBalance = tokenInfo.balance |> stringAsDecimal
    if tokenBalance > 0M then 
        PipelineProcessData.withProperty entity tokenInfo.symbol tokenBalance |> ignore

    let stakeBalance = tokenInfo.stake |> stringAsDecimal
    if stakeBalance > 0M then 
        PipelineProcessData.withProperty entity (tokenInfo.symbol+"_stake") stakeBalance |> ignore

    entity

let action apiUri (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    let username  = PipelineProcessData.readPropertyAsString entity "username"

    match username with 
    | Some username -> 
        getBalance apiUri username
        |> Seq.fold addTokenBalanceAsProperty entity
    | _ -> 
        NoUserDetails ModuleName 
        |> PipelineProcessData.withResult entity 

let bind hive (urls: Urls) (parameters: Map<string, string>) = 
    action urls.hiveEngineNodeUrl
