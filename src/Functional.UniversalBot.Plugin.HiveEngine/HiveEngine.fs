module HiveEngine

open System.Net.Http
open HiveEngineTypes
open HttpQuery

let contractsUri = sprintf "%s/contracts" 

let getBalance hiveEngineUrl username = 
    let payload = 
        {|
            contract = "tokens"
            table = "balances"
            query = {|
                account = username
            |}
        |}
    let result = runHiveQuery<RawTokenBalance seq> (contractsUri hiveEngineUrl) "find" (payload :> obj)
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

    runHiveQuery<RawMarketBuyBook seq> (contractsUri hiveEngineUrl) "find" (payload :> obj)
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

    runHiveQuery<RawPendingUnstakes seq> (contractsUri hiveEngineUrl) "find" (payload :> obj)
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
    |> Seq.collect (fun offset ->  runHiveQuery<RawLiqudityPools seq> hiveEngineUrl "find" (payload offset))
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
    |> Seq.collect (fun offset ->  runHiveQuery<RawTokenInfo seq> (contractsUri hiveEngineUrl) "find" (payload offset))
    |> Seq.map TokenInfo.bind
    |> Array.ofSeq
