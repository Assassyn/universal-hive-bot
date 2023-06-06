module BridgeAPI

open HttpQuery
open BridgeAPITypes

let getRankedPosts hiveUrl (sort: Sort) tag =
    runHiveQuery<RankedPost> hiveUrl "bridge.get_ranked_posts" {| sort = sort; tag = tag |}
