module HiveEngine

open System.Net.Http
open HiveAPI
open System.Text.Json
open FsHttp
open HiveEngineTypes

let private deserialize<'ResponsePayload> (json: JsonElement) = 
    JsonSerializer.Deserialize<HiveResponse<'ResponsePayload>> json

let private runContractsQuery<'ResponsePayload> hiveEngineUrl method (parameters: obj) = 
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
                   method = method
                   ``params`` = parameters
               |}
       }
       |> Request.send
       |> Response.toJson
       |> deserialize<'ResponsePayload>
       
   response.result
   
let getBalance hiveEngineUrl username = 
    let payload = 
        {|
            contract = "tokens"
            table = "balances"
            query = {|
                account = username
            |}
        |}
    let result = runContractsQuery<RawTokenBalance> hiveEngineUrl "find" (payload :> obj)
    result |> Seq.map TokenBalance.bind

let getMarketBuyBook hiveEngineUrl tokenSymbol =
    let payload = 
        {|
            contract = "market"
            query = {|
                symbol = tokenSymbol
            |}
            indexes = [
                {| 
                    index = "priceDec"
                    descending=true
                |}
            ]
            limit = 1000
            offset = 0
            table = "buyBook"
        |}

    runContractsQuery<RawMarketBuyBook> hiveEngineUrl "find" (payload :> obj)
    |> Seq.map MarketBuyBook.bind

let getPendingUnstakes hiveEngineUrl username =
    let payload = 
        {|
            contract = "tokens"
            table = "pendingUnstakes"
            query = {|
                account = username
            |}
        |}

    runContractsQuery<RawPendingUnstakes> hiveEngineUrl "find" (payload :> obj)
    |> Seq.map PendingUnstakes.bind
    |> Array.ofSeq


let getAvailableMarketPools hiveEngineUrl tokenPair = 
    let payload offset: obj = 
        {|  
            contract = "marketpools"
            table = "pools"
            query = {||}
            limit = 1000
            offset = offset
        |}

    seq { 0 .. 5 }
    |> Seq.map (fun x -> x * 1000)
    |> Seq.collect (fun offset ->  runContractsQuery<RawLiqudityPools> hiveEngineUrl "find" (payload offset))
    |> Seq.find (fun result -> result.tokenPair = tokenPair)
    |> LiqudityPools.bind


let getTokenDetails hiveEngineUrl = 
    let payload offset: obj = 
        {|  
            contract = "tokens"
            table = "tokens"
            query = {||}
            limit = 1000
            offset = offset
        |}
    
    seq { 0 .. 5 }
    |> Seq.map (fun x -> x * 1000)
    |> Seq.collect (fun offset ->  runContractsQuery<RawTokenInfo> hiveEngineUrl "find" (payload offset))
    |> Seq.map TokenInfo.bind
    |> Array.ofSeq
