module HttpQuery

open FsHttp
open HiveTypes
open HttpQuery

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
