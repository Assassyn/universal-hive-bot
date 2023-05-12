module HiveEngine

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
    runContractsQuery<TokenBalance> hiveEngineUrl "find" (payload :> obj)
    
type MarketBuyBook = 
    {
        _id: int64
        txId: string
        timestamp: int32
        account: string
        symbol: string
        quantity: string
        price: string
        priceDec: {|
            ``$numberDecimal``: string
        |}
        expiration: int64
    }
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

    runContractsQuery<MarketBuyBook> hiveEngineUrl "find" (payload :> obj)

type PendingUnstakes = 
    {
        _id: int64
        account: string
        symbol: string
        quantity: string
        quantityLeft: string
        nextTransactionTimestamp: int64
        numberTransactionsLeft: int16
        millisecPerPeriod: string
        txID: string
    }
let getPendingUnstakes hiveEngineUrl username tokenSymbol =
    let payload = 
        {|
            contract = "tokens"
            table = "pendingUnstakes"
            query = {|
                account = username
            |}
        |}

    runContractsQuery<PendingUnstakes> hiveEngineUrl "find" (payload :> obj)
