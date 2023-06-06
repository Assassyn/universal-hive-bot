module FlushTokens 

open PipelineResult
open Functional.ETL.Pipeline
open Types
open Functional.ETL.Pipeline.PipelineProcessData

[<Literal>]
let private ModuleName = "Flush"
 
let private isHiveOperation result = 
    match result with 
    | HiveOperation _ -> true
    | _ -> false

let private selectHiveOperationOnly result = 
    match result with 
    | HiveOperation (moduleRequesting, token, requiredKey, customJson) -> Some (moduleRequesting, token, requiredKey, customJson :> obj)
    | _ -> None

let private extractOperations entity =
    entity.results
    |> Seq.map selectHiveOperationOnly
    |> Seq.filter (fun x -> x.IsSome)
    |> Seq.map (fun x -> x.Value)
    |> Seq.groupBy (fun (_, _, requiredKey, _) -> requiredKey)

let private executeOperations hiveUrl keyRequired operations = 
    Hive.brodcastTransactions hiveUrl operations keyRequired 

let private extractCustomJson hiveOperationRequest = 
    let (_, _, _, customJson) = hiveOperationRequest
    customJson

let private processHiveOperations hiveUrl requiredKey key (operations: Map<KeyRequired,seq<Module * Token * KeyRequired * obj>>) = 
    if operations.ContainsKey (requiredKey)
    then 
        let ops =
            operations.[requiredKey] 
            |> Seq.map extractCustomJson
            |> Array.ofSeq 

        executeOperations hiveUrl key ops |> Array.ofSeq |> ignore

        operations.[requiredKey] 
        |> Seq.map (fun (moduleName, tokenSymbol, _, _) -> Processed (moduleName, tokenSymbol))
    else 
        Array.empty

let action hiveUrl (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    let userDetails: (string * string * string) option = PipelineProcessData.readPropertyAsType entity Readers.userdata 

    match userDetails with 
    | Some (username, activeKey, postingKey) -> 
        let operations = extractOperations entity |> Map.ofSeq

        let activeOperationResults = 
            operations |> processHiveOperations hiveUrl KeyRequired.Active activeKey 
        let postingOperationResults =
            operations |> processHiveOperations hiveUrl KeyRequired.Posting postingKey

        let results = 
            entity.results 
            |> Seq.filter (fun x -> not (isHiveOperation x))
            |> Seq.append activeOperationResults 
            |> Seq.append postingOperationResults 
            |> List.ofSeq

        { entity with results = results }
        |>= FlushingFinshed (username, results)
    | _ -> 
        NoUserDetails ModuleName <=< entity

let bind urls (parameters: Map<string, string>) = 
    action urls.hiveNodeUrl
