module PostId

open System
open BridgeAPITypes
open PipelineProcessData
open Pipeline

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
let bindPost (rankedPost: Post) = 
    {
        id = rankedPost.post_id
        author = rankedPost.author
        category = rankedPost.category
        permlink = rankedPost.permlink
        created = DateTimeOffset.Parse(rankedPost.created)
        voters = rankedPost.active_votes |> Seq.map (fun votes -> votes.voter)
    }

let bindParentPost (rankedPost: Post) = 
    {
        id = rankedPost.post_id
        author = rankedPost.parent_author
        category = rankedPost.category
        permlink = rankedPost.parent_permlink
        created = DateTimeOffset.Parse(rankedPost.created)
        voters = rankedPost.active_votes |> Seq.map (fun votes -> votes.voter)
    }
