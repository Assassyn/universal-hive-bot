module HttpQuery

open FsHttp
open Json
open HiveTypes

let getQuery<'ResponsePayload> requestUri = 
    http {
        GET requestUri
        CacheControl "no-cache"
    }
    |> Request.send

let postQuery<'ResponsePayload> requestUri (requestBody: obj) = 
    http {
        POST requestUri
        CacheControl "no-cache"
        body
        jsonSerialize requestBody
    }
    |> Request.send
    
module Response = 
    let deserializeFromJson<'ResponsePayload> (response: Domain.Response) =
        response
        |> Response.toJson
        |> deserialize<'ResponsePayload>

    let private intMaxValue = 
        Some System.Int32.MaxValue

    let returnString (response: Domain.Response) =
        response
        |> Response.toString intMaxValue


let runHiveQuery<'ResponsePayload> requestUri method (parameters: obj) = 
    let body = 
        {|
            jsonrpc = "2.0"
            id = 1
            method = method
            ``params`` = parameters
        |}
    postQuery<HiveResponse<'ResponsePayload>> requestUri body
    |> Response.deserializeFromJson<HiveResponse<'ResponsePayload>>
    |> fun response -> response.result

let emptyParameters = Array.empty
