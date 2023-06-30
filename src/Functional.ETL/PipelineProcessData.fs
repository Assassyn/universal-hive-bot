module PipelineProcessData 

open Pipeline
open System.Text.RegularExpressions

let withProperty entity key value =
    let properties = entity.properties.Add (key, value :> obj)
    { entity with properties = properties }

let addProperty key value entity =
    withProperty entity key value

let (|>+) entity key value = 
    withProperty entity key (value :> obj) 

let withResult<'Result> (entity: PipelineProcessData<'Result>) (value: 'Result) =
    let results = value::entity.results
    { entity with results = results }
    
let (|>=) entity result = withResult entity result
let (|=>) result entity = withResult entity result   
let (>=>) entity result = withResult entity result   
let (<=<) result entity = withResult entity result   

let readProperty entity key =
    match entity.properties.ContainsKey (key) with 
    | true -> Some entity.properties.[key]
    | _ -> None

let (=>) entity key = readProperty entity key

let readPropertyAsType<'EntityResult, 'PropertyType> (entity: PipelineProcessData<'EntityResult>) key =
    let property = readProperty entity key
    match property with 
    | Some x -> Some (x :?> 'PropertyType)
    | _ -> None
let readPropertyAsString<'Result> = readPropertyAsType<'Result, string>
let readPropertyAsDecimal<'Result> = readPropertyAsType<'Result, decimal>

let hasProperty key entity = 
    entity.properties.ContainsKey key

let getProperty key entity =
    readProperty entity key

let bind index = 
    let entity = 
        {
            index = index
            properties = Map.empty
            results = list.Empty
        }
    entity
let bind32 index =
    bind (int64(index))

let enumerateProperties<'EntityResult, 'PropertyType> (label: string) (entity: PipelineProcessData<'EntityResult>) = 
    let isMatch (key: string) = 
        let pattern = $"{label}_\d{{1,}}"
        Regex.IsMatch (key, pattern)
    entity.properties.Keys
    |> Seq.filter isMatch 
    |> Seq.map (readPropertyAsType<'EntityResult, 'PropertyType> entity)
    |> Seq.filter (fun optionItem -> optionItem.IsSome)
    |> Seq.map (fun optionItem -> optionItem.Value)
