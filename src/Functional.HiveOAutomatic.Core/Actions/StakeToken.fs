namespace Functioanl.HiveBot.HIVEConverter

module StakeToken =
    open Layer2
    open Functional.ETL.Pipeline

    let private getJson (hive: Hive.Hive) tokenStakingDetails =
        let (username, symbol, quantity) = tokenStakingDetails
        
        let json =
            sprintf 
                """{"contractName":"tokens","contractAction":"stake","contractPayload": {"to": "%s","symbol": "%s","quantity": "%M"}}""" 
                username 
                symbol 
                quantity
        let operations = hive.createCustomJsonActiveKey username "ssc-mainnet-hive" json
        operations


    let getTokenDetails entity username tokensName = 
        let balance = 
            match (PipelineProcessData.readPropertyAsString entity tokensName) with 
            | Some x -> System.Decimal.Parse x
            | _ -> 0M
        (username, tokensName, balance)

    let stakeTokens (hive: Hive.Hive) activeKey operations = 
        try 
            let txid = hive.brodcastTransaction operations activeKey
            ()
        with  
            | ex -> 
                let msg = ex.Message
                ()
        ()

    let ignoreEmpty tokenStakingDetails =
        let (_, _, quantity) = tokenStakingDetails
        quantity > 0.0M 

    let action (hive: Hive.Hive) tokensToStake (entity: PipelineProcessData) = 
        let userDetails = PipelineProcessData.readPropertyAsType<string * string * string> entity "userdata"

        match userDetails with 
        | Some (username, activeKey, _) -> 
            tokensToStake
            |> Seq.map (getTokenDetails entity username)
            |> Seq.filter ignoreEmpty
            |> Seq.map (getJson hive)
            |> Seq.iter (stakeTokens hive activeKey)
            |> ignore
        | _ -> 
            ()
        entity