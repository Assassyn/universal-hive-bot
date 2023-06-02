module Hive

open System.Net.Http
open HiveAPI
open System

type HiveUrl = string 

[<Literal>]
let useActiveKey = true

[<Literal>]
let usePostingKey = true

let private setToUserNameWhenTrue username isTrue =
    match isTrue with 
    | true -> [| username |]
    | _ -> [||]

let private createCustomJson username activeKey postingKey methodName json = 
    new COperations.custom_json (
        id = methodName,
        json = json,
        required_auths = setToUserNameWhenTrue username activeKey,
        required_posting_auths = setToUserNameWhenTrue username postingKey)

let brodcastTransactions hiveUrl operations key = 
    let hive = new CHived(new HttpClient(), hiveUrl)
    operations
    |> Seq.chunkBySize 5
    |> Seq.map (fun op -> 
        let transactionId = hive.broadcast_transaction (op, [| key |])
        System.Threading.Thread.Sleep (3 |> TimeSpan.FromSeconds)
        transactionId)

let getMessages hiveUrl = 
    let hive = new CHived(new HttpClient(), hiveUrl)
    hive.get_feed_history ()

let buildActiveKeyedCustomJson username method payload = 
    let json = System.Text.Json.JsonSerializer.Serialize (payload)
    createCustomJson username useActiveKey (not usePostingKey) method json

let buildPostingKeyedCustomJson username method payload = 
    let json = System.Text.Json.JsonSerializer.Serialize (payload)
    createCustomJson username (not useActiveKey) usePostingKey method json
