module TemplateAPI

open HttpQuery

let private castSort sort = 
    sort.ToString().ToLower()

[<Literal>]
let private gist = "https://gist.githubusercontent.com/"

let getTemplate url =
    sprintf "%s%s" gist url
    |> getQuery<string> 
    |> Response.returnString
