module HttpQuery

open FsHttp
open Json

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
