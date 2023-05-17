module HiveEngine

open System.Net.Http
open HiveAPI
open System.Text.Json
open FsHttp
open FunctionalString
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
    runContractsQuery<RawTokenBalance> hiveEngineUrl "find" (payload :> obj)
    |> Seq.map TokenBalance.bind

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
            limit = 100
            offset = 0
            table = "buyBook"
        |}

    runContractsQuery<RawMarketBuyBook> hiveEngineUrl "find" (payload :> obj)
    |> Seq.map MarketBuyBook.bind

let getPendingUnstakes hiveEngineUrl username tokenSymbol =
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
    |> Seq.filter (fun token -> token.symbol = tokenSymbol)
