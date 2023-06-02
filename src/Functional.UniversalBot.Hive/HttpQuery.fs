module HttpQuery

open FsHttp
open Json
open HiveTypes

let runHiveQuery<'ResponsePayload> requestUri method (parameters: obj) = 
   let response = 
       http {
           POST requestUri
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
       |> deserialize<HiveResponse<'ResponsePayload>>
       
   response.result

let emptyParameters = Array.empty
