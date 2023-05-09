module TestingStubs

open Functional.ETL.Pipeline

let logger a b =
    ()

let mockedBalanceAction balanceLevles entity = 
    balanceLevles
    |> Seq.fold (fun entity (tokenSymbol, tokenBalance) -> PipelineProcessData.withProperty entity tokenSymbol tokenBalance) entity

let mockedStakedBalanceAction balanceLevles entity = 
    let stakedTokenSymbole = sprintf "%s_stake"
    balanceLevles
    |> Seq.fold (fun entity (tokenSymbol, tokenBalance) -> PipelineProcessData.withProperty entity (stakedTokenSymbole tokenSymbol) tokenBalance) entity
    
let mockedDelegatedStakedBalanceAction balanceLevles entity = 
    let stakedTokenSymbole = sprintf "%s_delegatedstake"
    balanceLevles
    |> Seq.fold (fun entity (tokenSymbol, tokenBalance) -> PipelineProcessData.withProperty entity (stakedTokenSymbole tokenSymbol) tokenBalance) entity
    
let extractCustomJson underTestObject =
    match underTestObject with 
    | PipelineResult.HiveOperation (_, _, _, customJson) -> customJson.json
    | _ -> ""

let inline (~~) x = x :> obj
