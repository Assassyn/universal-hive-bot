namespace Functioanl.HiveBot.HIVEConverter

module LoadLevel2Tokens =
    open Layer2
    open Functional.ETL.Pipeline

    let private addTokenBalanceAsProperty entity tokenBalance =
        PipelineProcessData.withProperty entity tokenBalance.symbol tokenBalance.balance

    let action apiUri (entity: PipelineProcessData) = 
        let userdata = PipelineProcessData.readPropertyAsType<string * string * string> entity "userdata"

        match userdata with 
        | Some (username, _, _) -> 
            getBalance apiUri username
            |> Seq.fold addTokenBalanceAsProperty entity
        | _ -> 
            entity

//let getUserReader (usernames: string seq) =
//    usernames
//    |> Seq.mapi createEntity
//    |> TaskSeq.ofSeq

