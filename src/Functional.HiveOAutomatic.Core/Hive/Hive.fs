module Hive

open System
open System.Net.Http
open HiveAPI

type Hive (hiveNodeUrl) = 
    let hive = new CHived(new HttpClient(), hiveNodeUrl)
    let applicationIdentifier = "hive-bot"

    let generateRandomString numebrOfCharacters = 
           let randomizer = Random()
           let chars = "0123456789abcdefghijklmnopqrstuvwxyz".ToCharArray()
           let sz = Array.length chars in
           String(Array.init numebrOfCharacters (fun _ -> chars.[randomizer.Next sz])).ToString()

    let setToUserNameWhenTrue username isTrue =
        match isTrue with 
        | true -> [| username |]
        | _ -> [||]

    let createCustomJson username activeKey postingKey methodName json = 
        new COperations.custom_json (
            id = methodName,
            json = json,
            required_auths = setToUserNameWhenTrue username activeKey,
            required_posting_auths = setToUserNameWhenTrue username postingKey)
    
    member this.createCustomJsonActiveKey username methodName getJson = 
        let json = getJson applicationIdentifier (generateRandomString 10)
        createCustomJson username true false methodName json
    
    member this.createCustomJsonPostingKey username methodName getJson = 
        let json = getJson applicationIdentifier (generateRandomString 10)
        createCustomJson username false true methodName json
    
    member this.createTransaction json key =
        hive.create_transaction([| json |], [| key |])
    
    member this.brodcastTransaction operations key = 
        hive.broadcast_transaction([| operations |] , [| key |])

//let private hive = new CHived(new HttpClient(), hiveNodeUrl)

//let private setToUserNameWhenTrue username isTrue =
//    match isTrue with 
//    | true -> [| username |]
//    | _ -> [||]

//let private createCustomJson username activeKey postingKey methodName json = 
//    new COperations.custom_json (
//        id = methodName,
//        json = json,
//        required_auths = setToUserNameWhenTrue username activeKey,
//        required_posting_auths = setToUserNameWhenTrue username postingKey)

//let createCustomJsonActiveKey username methodName getJson = 
//    let json = getJson applicationIdentifier (Randomizer.generateRandomString 10)
//    createCustomJson username true false methodName json

//let createCustomJsonPostingKey username methodName getJson = 
//    let json = getJson applicationIdentifier (Randomizer.generateRandomString 10)
//    createCustomJson username false true methodName json

//let createTransaction json key =
//    hive.create_transaction([| json |], [| key |])

//let brodcastTransaction operations key = 
//    hive.broadcast_transaction([| operations |] , [| key |])
    
//type TransactionId = string
//type TransactionResult = 
//    | FinishedOk of TransactionId
//    | DidNotFinished of TransactionId
        
//let waitForTransaction playerName transactionId =
//    let sleepTime = TimeSpan.FromSeconds 5.0
//    let mutable counter = 25
//    while counter > 0 do 
//        let transactions = hive.get_account_history (playerName, int64 -1, uint32 3)
//        let containsTransaction = 
//            transactions.Children()
//            |> Seq.map (fun token -> token.Last)
//            |> Seq.map (fun trans -> trans.["trx_id"].ToString() )
//            |> Array.ofSeq
//            |> Array.contains transactionId

//        if  not containsTransaction 
//        then 
//            Threading.Thread.Sleep sleepTime
//            counter <- counter - 1 
//        else 
//            counter <- 0

//    match counter with 
//    | x when x = 0 -> DidNotFinished transactionId
//    | _  -> FinishedOk transactionId
