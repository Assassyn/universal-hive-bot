module TerracoreAPI

open TerracoreTypes
open Json
open FsHttp
open System.Text.Json

let terracoreAPI part = 
    sprintf "http://terracore.herokuapp.com/%s" part

let private httpRequest<'ResponsePayload> uri = 
    let response = 
        http {
            GET uri
        }
        |> Request.send
        |> Response.toJson
        |> deserialize<'ResponsePayload>
    response

let getPlayerInfo playerName = 
    let uri = terracoreAPI (sprintf "player/%s" playerName)
    let response = httpRequest<Player> uri
    response

//let hash () =
//    let random = System.Math.Ran
//    //var hash = Math.random().toString(36).substring(2, 15) + Math.random().toString(36).substring(2, 15);
