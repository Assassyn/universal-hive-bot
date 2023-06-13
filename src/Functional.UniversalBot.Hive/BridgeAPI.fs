﻿module BridgeAPI

open HttpQuery
open BridgeAPITypes

let private castSort sort = 
    sort.ToString().ToLower()

let getRankedPosts hiveUrl (sort: Sort) tag =
    runHiveQuery<RankedPost seq> hiveUrl "bridge.get_ranked_posts" {| sort = sort |> castSort; tag = tag |}

let getDiscussion hiveUrl author permlink = 
    runHiveQuery<Map<string, Discussion>> hiveUrl "bridge.get_discussion" {| author  = author; permlink = permlink|}
