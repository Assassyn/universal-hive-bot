namespace Functioanl.HiveBot.HIVEConverter

module StakeToken =
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
        (operations, symbol)


    let getTokenDetails entity username tokensName = 
        let balance = 
            match (PipelineProcessData.readPropertyAsString entity tokensName) with 
            | Some x -> System.Decimal.Parse x
            | _ -> 0M
        (username, tokensName, balance)

    let stakeTokens (hive: Hive.Hive) activeKey token operation entity = 
        try 
            let txid = hive.brodcastTransaction operation activeKey
            
            PipelineProcessData.withResult entity (Pipeline.UniversalHiveBotResutls.Processed ("StakeAction", token))
        with  
            | ex -> 
                let msg = ex.Message
                PipelineProcessData.withResult entity (Pipeline.UniversalHiveBotResutls.UnableToProcess ("StakeAction", token, msg))

    let ignoreEmpty tokenStakingDetails =
        let (_, _, quantity) = tokenStakingDetails
        quantity > 0.0M 

    let action (hive: Hive.Hive) tokensToStake (entity: PipelineProcessData<Pipeline.UniversalHiveBotResutls>) = 
        let userDetails = PipelineProcessData.readPropertyAsType entity "userdata"

        match userDetails with 
        | Some (username, activeKey, _) -> 
            tokensToStake
            |> Seq.map (getTokenDetails entity username)
            |> Seq.filter ignoreEmpty
            |> Seq.map (getJson hive)
            |> Seq.fold (fun entity (operation, token) -> stakeTokens hive activeKey token operation entity) entity 
        | _ -> 
            PipelineProcessData.withResult entity (Pipeline.UniversalHiveBotResutls.NoUserDetails "StakeAction")