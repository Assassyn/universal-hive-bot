module HiveTypes

type HiveResponse<'Result> =
    {
        jsonrpc: string
        id: int64
        result: 'Result
    }

type Properties = 
    {
        head_block_number: int64
        head_block_id: string
        time: string
        current_witness: string
        total_pow: int64
        num_pow_witnesses: int64
        virtual_supply: string
        current_supply: string
        init_hbd_supply: string
        current_hbd_supply: string
        total_vesting_fund_hive: string
        total_vesting_shares: string
        total_reward_fund_hive: string
        total_reward_shares2: string
        pending_rewarded_vesting_shares: string
        pending_rewarded_vesting_hive: string
        hbd_interest_rate: int64
        hbd_print_rate: int64
        maximum_block_size: int64
        required_actions_partition_percent: int64
        current_aslot: int64
        recent_slots_filled: string
        participation_count: int64
        last_irreversible_block_num: int64
        vote_power_reserve_rate: int64
        delegation_return_period: int64
        reverse_auction_seconds: int64
        available_account_subsidies: int64
        hbd_stop_percent: int64
        hbd_start_percent: int64
        next_maintenance_time: string
        last_budget_time: string
        next_daily_maintenance_time: string
        content_reward_percent: int64
        vesting_reward_percent: int64
        proposal_fund_percent: int64
        dhf_interval_ledger: string
        downvote_pool_percent: int64
        current_remove_threshold: int64
        early_voting_seconds: int64
        mid_voting_seconds: int64
        max_consecutive_recurrent_transfer_failures: int64
        max_recurrent_transfer_end_date: int64
        min_recurrent_transfers_recurrence: int64
        max_open_recurrent_transfers: int64
    }
