namespace Functioanl.HiveBot.HIVEConverter

module LoadLevel2Tokens =
    open Layer2
    open Functional.ETL.Pipeline

    let private addTokenBalanceAsProperty entity tokenBalance =
        PipelineProcessData.withProperty entity tokenBalance.symbol tokenBalance.balance

    let LoadLevel2Tokens apiUri (entity: PipelineProcessData) = 
        let username = PipelineProcessData.readPropertyAsString entity "username"

        match username with 
        | Some username -> 
            getBalance apiUri username
            |> Seq.fold addTokenBalanceAsProperty entity
        | _ -> 
            entity

//let getUserReader (usernames: string seq) =
//    usernames
//    |> Seq.mapi createEntity
//    |> TaskSeq.ofSeq

