namespace Functioanl.HiveBot.HIVEConverter

module LoadLevel2Tokens =
    open Functional.ETL.Pipeline

    let LoadLevel2Tokens (entity: PipelineProcessData) = 
        let username = PipelineProcessData.readProperty entity "username"
        entity

//let getUserReader (usernames: string seq) =
//    usernames
//    |> Seq.mapi createEntity
//    |> TaskSeq.ofSeq

