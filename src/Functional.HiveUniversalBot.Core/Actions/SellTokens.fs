module SellToken 

open PipelineResult
open Types
open Functional.ETL.Pipeline

[<Literal>]
let ModuleName = "Sell"

//{
//   "contractName": "market",
//   "contractAction": "sell",
//   "contractPayload": {
//      "symbol": "VOCUP",
//      "quantity": "0.2",
//      "price": "1.0000015"
//   }
//}

let action logger hive hiveEngineUrl tokenSymbol (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    ()

let bind logger (urls: Urls) (parameters: Map<string, string>) = 
    let token = parameters.["token"]
    action logger urls.hiveNodeUrl urls.hiveEngineNodeUrl token
