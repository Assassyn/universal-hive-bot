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
        //"replies": [],
        //"active_votes": [{"voter": "bob", "rshares": "67759296290"}],
        author_reputation: decimal
        //"stats": {
        //  "hide": false,
        //  "gray": false,
        //  "total_votes": 12,
        //  "flag_weight": 0
        //},
        //"beneficiaries": [],
        //"max_accepted_payout": "1000000.000 HBD",
        percent_hbd: int32
        //"url": "/wonderland/@alice/that-march-hare",
        //"blacklists": []
      }
