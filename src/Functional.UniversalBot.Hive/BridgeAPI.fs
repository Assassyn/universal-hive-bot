﻿module BridgeAPI

open HttpQuery
open BridgeAPITypes

let private castSort sort = 
    sort.ToString().ToLower()

let getRankedPosts hiveUrl (sort: Sort) tag =
    runHiveQuery<RankedPost seq> hiveUrl "bridge.get_ranked_posts" {| sort = sort |> castSort; tag = tag |}

let getDiscussion hiveUrl author permlink = 
    runHiveQueryAsync<Map<string, Discussion>> hiveUrl "bridge.get_discussion" {| author  = author; permlink = permlink|}

let getAccountComments hiveUrl account limit = 
    runHiveQuery<Post seq> hiveUrl "bridge.get_account_posts" {| sort = "comments"; account = account; limit = limit |}

let getPost hiveUrl author permanentLink = 
    runHiveQuery<Post> hiveUrl "bridge.get_post" {| author = author; permlink = permanentLink |} 
