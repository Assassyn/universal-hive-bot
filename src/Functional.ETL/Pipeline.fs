namespace Functional.ETL

open FSharp.Control

module Pipeline =
    type PipelineProcessData<'Result> = 
        {
            index: int64
            properties: Map<string, obj>
            results: 'Result list
        }
    type WriterResult = 
        | Success
        | Failure
    type ErrorMessage = string
    type Reader<'Result> = unit -> PipelineProcessData<'Result> taskSeq
    type Transformer<'Result> = PipelineProcessData<'Result> -> PipelineProcessData<'Result>
        
    module PipelineProcessData = 
        let withProperty entity key value =
            let properties = entity.properties.Add (key, value)
            { entity with properties = properties }
        let addProperty key value entity =
            withProperty entity key value
        let withResult<'Result> (entity: PipelineProcessData<'Result>) (value: 'Result) =
            let results = value::entity.results
            { entity with results = results }
        let readProperty entity key =
            match entity.properties.ContainsKey (key) with 
            | true -> Some entity.properties.[key]
            | _ -> None

        let (=>) entity key = readProperty entity key

        let readPropertyAsType<'EntityResult, 'PropertyType> (entity: PipelineProcessData<'EntityResult>) key =
            let property = readProperty entity key
            match property with 
            | Some x -> Some (x:?> 'PropertyType)
            | _ -> None
        let readPropertyAsString<'Result> = readPropertyAsType<'Result, string>
        let readPropertyAsDecimal<'Result> = readPropertyAsType<'Result, decimal>

        let bind index = 
            {
                index = index
                properties = Map.empty
                results = list.Empty
            }

    module Transformer = 
        let empty<'Error> =
            fun entity -> Ok entity
        let defaultTransformer<'Result> (entity: PipelineProcessData<'Result>) = entity

    type Pipeline<'Results> = 
        {
            extractor: Reader<'Results>
            transformers: Transformer<'Results>
        }
    module Pipeline = 
        let bind reader transformers = 
            { 
                extractor = reader 
                transformers = transformers
            }

    let processPipeline pipelineDefinition =     
        pipelineDefinition.extractor ()
        |> TaskSeq.map pipelineDefinition.transformers
        |> TaskSeq.toArray
        
