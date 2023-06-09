module PostId

open BridgeAPITypes
open System

type PostIdentification = 
    {
        id: int64
        author: string
        permlink: string
        category: string
        created: DateTimeOffset
        voters: string seq
    }

let bind (rankedPost: RankedPost) = 
    {
        id = rankedPost.post_id
        author = rankedPost.author
        category = rankedPost.category
        permlink = rankedPost.permlink
        created = DateTimeOffset.Parse(rankedPost.created)
        voters = rankedPost.active_votes |> Seq.map (fun votes -> votes.voter)
    }
