namespace Functioanl.HiveBot.HIVEConverter

module LoadLevel2Tokens =
    open Layer2
    open Functional.ETL.Pipeline

    let private addTokenBalanceAsProperty entity tokenBalance =
        PipelineProcessData.withProperty entity tokenBalance.symbol tokenBalance.balance

    let action<'Result> apiUri (entity: PipelineProcessData<Pipeline.UniversalHiveBotResutls>) = 
        let userdata = PipelineProcessData.readPropertyAsType<Pipeline.UniversalHiveBotResutls, string * string * string> entity "userdata"

        match userdata with 
        | Some (username, _, _) -> 
            getBalance apiUri username
            |> Seq.fold addTokenBalanceAsProperty entity
        | _ -> 
            PipelineProcessData.withResult entity (Pipeline.UniversalHiveBotResutls.NoUserDetails "LoadTokens")
