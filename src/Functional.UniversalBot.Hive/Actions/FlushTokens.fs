module FlushTokens 

open PipelineResult
open Pipeline
open Types
open PipelineProcessData
open Hive

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

let private extractCustomJson hiveOperationRequest = 
    let (_, _, _, customJson) = hiveOperationRequest
    customJson

//let private getChunkSizeBasedOnKey key = 
//    match key with
//    | Active | Posting -> ChunkSize.MaxCountInBlock
//    | _ -> ChunkSize.Single

let private getByKey requiredKey (operations: Map<KeyRequired,seq<Module * Token * KeyRequired * obj>>) = 
    if operations.ContainsKey (requiredKey)
    then 
        operations.[requiredKey] 
        |> Seq.map extractCustomJson
        |> Array.ofSeq
    else 
        Array.empty
        
let private mapperToProcessed transactionId = 
    Processed ("TransactionId", transactionId)

let private processHiveOperations hiveUrl chunkSize key ops = 
    try 
        Hive.brodcastTransactions hiveUrl chunkSize ops key 
        |> Array.ofSeq 
        |> Array.map mapperToProcessed 
    with 
        | ex -> [| Error ex.Message |]


let action hiveUrl (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    task {
        let userDetails: (string * string * string) option = PipelineProcessData.readPropertyAsType entity Readers.userdata 

        return 
            match userDetails with 
            | Some (username, activeKey, postingKey) -> 
                let operations = extractOperations entity |> Map.ofSeq

                let activeOperationResults = 
                    operations
                    |> getByKey KeyRequired.Active
                    |> processHiveOperations hiveUrl ChunkSize.MaxCountInBlock activeKey
                let activeSingleOperationResults = 
                    operations
                    |> getByKey KeyRequired.ActiveSingle
                    |> processHiveOperations hiveUrl ChunkSize.Single activeKey
                let postingOperationResults =
                    operations
                    |> getByKey KeyRequired.Posting
                    |> processHiveOperations hiveUrl ChunkSize.MaxCountInBlock postingKey
                let postingSingleOperationResults =
                    operations
                    |> getByKey KeyRequired.PostingSingle
                    |> processHiveOperations hiveUrl ChunkSize.Single postingKey

                let results = 
                    entity.results 
                    |> Seq.filter (fun x -> not (isHiveOperation x))
                    |> Seq.append (activeOperationResults)
                    |> Seq.append (activeSingleOperationResults)
                    |> Seq.append (postingOperationResults)
                    |> Seq.append (postingSingleOperationResults)
                    |> List.ofSeq

                { entity with results = results }
                |>= FlushingFinshed (username, results)
            | _ -> 
                NoUserDetails ModuleName <=< entity
    }

let bind urls (parameters: Map<string, string>) = 
    action urls.hiveNodeUrl
