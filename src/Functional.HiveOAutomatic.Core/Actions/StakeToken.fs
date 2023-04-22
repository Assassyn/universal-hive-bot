namespace Functioanl.HiveBot.HIVEConverter

module StakeToken =
    open Layer2
    open Functional.ETL.Pipeline

    let private getJson username symbol quantity =
        sprintf 
            """{
            "contractName":"tokens",
            "contractAction":"stake",
            "contractPayload": {
                "to": "%s",
                "symbol": "%s",
                "quantity": "%s"
            }
            }""" 
            username 
            symbol 
            quantity

    let action hive tokensToStake (entity: PipelineProcessData) = 
        entity

//let getUserReader (usernames: string seq) =
//    usernames
//    |> Seq.mapi createEntity
//    |> TaskSeq.ofSeq

