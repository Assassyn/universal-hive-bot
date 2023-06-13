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

let createVote voter author permlink weight = 
    new COperations.vote (
        author = author,
        permlink = permlink,
        voter = voter,
        weight = weight)

let createComment author body json_metadata parent_author parent_permlink permlink title =
    new COperations.comment (
        author = author, 
        body = body, 
        json_metadata = json_metadata, 
        parent_author = parent_author, 
        parent_permlink = parent_permlink, 
        permlink = permlink, 
        title = title)

type ChunkSize = 
    | Single = 1
    | MaxCountInBlock = 5

let brodcastTransactions hiveUrl (chunkSize: ChunkSize) operations key = 
    let hive = new CHived(new HttpClient(), hiveUrl)
    operations
    |> Seq.chunkBySize (int chunkSize)
    |> Seq.map (fun op -> 
        let transactionId = hive.broadcast_transaction (op, [| key |])
        System.Threading.Thread.Sleep (3 |> TimeSpan.FromSeconds)
        transactionId)

let buildActiveKeyedCustomJson username method payload = 
    let json = System.Text.Json.JsonSerializer.Serialize (payload)
    createCustomJson username useActiveKey (not usePostingKey) method json

let buildPostingKeyedCustomJson username method payload = 
    let json = System.Text.Json.JsonSerializer.Serialize (payload)
    createCustomJson username (not useActiveKey) usePostingKey method json
