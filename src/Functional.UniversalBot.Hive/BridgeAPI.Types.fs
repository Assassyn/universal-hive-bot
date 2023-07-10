module BridgeAPITypes
 
type Sort = 
    | Trending 
    | Hot
    | Created
    | Promoted
    | Payout
    | Payout_comments
    | Muted

type JsonMetadata =
    {
        tags: string seq
        app: string
    }
    
type ActiveVotes = 
    {
        voter: string 
        rshares: int64
    }

type RankedPost = 
    {
        post_id: int64
        author: string
        permlink: string
        category: string
        title: string
        body: string
        json_metadata: JsonMetadata
        created: string
        updated: string
        depth: int64
        children: int64
        net_rshares: int64
        is_paidout: bool
        payout_at: string
        payout: decimal
        pending_payout_value: string
        author_payout_value: string
        curator_payout_value: string
        promoted: string
        active_votes: ActiveVotes seq
        author_reputation: decimal
        percent_hbd: int32
        parent_author: string
        parent_permlink: string
      }

type Post = 
    {
        post_id: int64
        author: string
        permlink: string
        category: string
        title: string
        body: string
        json_metadata: JsonMetadata
        created: string
        updated: string
        depth: int64
        children: int64
        net_rshares: int64
        is_paidout: bool
        payout_at: string
        payout: decimal
        pending_payout_value: string
        author_payout_value: string
        curator_payout_value: string
        promoted: string
        active_votes: ActiveVotes seq
        author_reputation: decimal
        percent_hbd: int32
        parent_author: string
        parent_permlink: string
    }

type Discussion = 
    {
        post_id: int64
        author: string 
        //permlink        author
        //category        author
        //title        author
        //bodyauthor
        //"json_metadata": {
        //  "tags": ["hiveecosystem"],
        //  "users": ["blocktrades", "howo"],
        //  "image": [
        //    "https://images.hive.blog/768x0/https://files.peakd.com/file/peakd-hive/hiveio/pKjrNcbK-Hive-Wallpaper-1920x1080.png",
        //    "https://images.hive.blog/DQmR3iwCn9yvwXDXfuNjmMX6FrjAvFfYQWgA4QRckpens1j/hive%20dividers-02.png"
        //  ],
        //  "links": [
        //    "https://gitlab.syncad.com/hive/hive-whitepaper/-/blob/master/technical-vision/infographic.pdf"
        //  ],
        //  "app": "hiveblog/0.1",
        //  "format": "markdown",
        //  "description": "The strength of Hive lies in our decentralization."
        //},
        //"created": "2021-02-14T08:16:03",
        //"updated": "2021-02-14T08:16:03",
        //"depth": 0,
        //"children": 15,
        //"net_rshares": 93531156115025,
        //"is_paidout": true,
        //"payout_at": "2021-02-21T08:16:03",
        //"payout": 0,
        //"pending_payout_value": "0.000 HBD",
        //"author_payout_value": "0.000 HBD",
        //"curator_payout_value": "0.000 HBD",
        //"promoted": "0.000 HBD",
        //"replies": [],
        //"author_reputation": 69.29,
        //"stats": {
        //  "hide": false,
        //  "gray": false,
        //  "total_votes": 129,
        //  "flag_weight": 0
        //},
        //"url": "/hiveecosystem/@hiveio/around-the-hive-reflections",
        //"beneficiaries": [],
        //"max_accepted_payout": "0.000 HBD",
        //"percent_hbd": 10000,
        //"active_votes": [],
        //"blacklists": []
      }
