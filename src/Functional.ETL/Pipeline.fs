namespace Functional.ETL

open FSharp.Control

module Pipeline =
    type PipelineProcessData = 
        {
            index: int64
            properties: Map<string, obj>
        }
    type WriterResult = 
        | Success
        | Failure
    type ErrorMessage = string
    type Reader<'Error> = unit -> Result<PipelineProcessData, 'Error> taskSeq
    type Transformer<'Error> = Result<PipelineProcessData, 'Error> -> Result<PipelineProcessData, 'Error>    
        
    module PipelineProcessData = 
        let withProperty entity key value =
            let properties = entity.properties.Add (key, value)
            { entity with properties = properties }
        let readProperty entity key =
            match entity.properties.ContainsKey (key) with 
            | true -> Some entity.properties.[key]
            | _ -> None
        let readPropertyAsType<'TResult> entity key =
            let property = readProperty entity key
            match property with 
            | Some x -> Some (x:?> 'TResult)
            | _ -> None
        let readPropertyAsString = readPropertyAsType<string>

        let bind index = 
            {
                index = index
                properties = Map.empty
            }

    module Transformer = 
        let empty<'Error> =
            fun entity -> Ok entity
        let wrap (transformFunction: PipelineProcessData -> PipelineProcessData) (result: Result<PipelineProcessData, 'Error>) = 
            match result with 
            | Ok entity -> entity |> transformFunction |> Ok
            | Error error -> Error error

    type Pipeline<'Error> = 
        {
            extractor: Reader<'Error>
            transformers: Transformer<'Error>
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
        |> TaskSeq.toSeq
        
