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
        parent_author: string
        parent_permlink: string
    }

let bind (rankedPost: RankedPost) = 
    {
        id = rankedPost.post_id
        author = rankedPost.author
        category = rankedPost.category
        permlink = rankedPost.permlink
        created = DateTimeOffset.Parse(rankedPost.created)
        voters = rankedPost.active_votes |> Seq.map (fun votes -> votes.voter)
        parent_author = rankedPost.parent_author
        parent_permlink = rankedPost.parent_permlink
    }
let bindPost (rankedPost: Post) = 
    {
        id = rankedPost.post_id
        author = rankedPost.author
        category = rankedPost.category
        permlink = rankedPost.permlink
        created = DateTimeOffset.Parse(rankedPost.created)
        voters = rankedPost.active_votes |> Seq.map (fun votes -> votes.voter)
        parent_author = rankedPost.parent_author
        parent_permlink = rankedPost.parent_permlink
    }

let bindParentPost (rankedPost: Post) = 
    {
        id = rankedPost.post_id
        author = rankedPost.parent_author
        category = rankedPost.category
        permlink = rankedPost.parent_permlink
        created = DateTimeOffset.Parse(rankedPost.created)
        voters = rankedPost.active_votes |> Seq.map (fun votes -> votes.voter)
        parent_author = rankedPost.parent_author
        parent_permlink = rankedPost.parent_permlink
    }
