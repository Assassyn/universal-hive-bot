namespace Functional.ETL

open FSharp.Control
open System.Threading.Tasks

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
    type Transformer<'Result> = PipelineProcessData<'Result> -> Task<PipelineProcessData<'Result>>

        
    module PipelineProcessData = 
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
            | Some x -> Some (x:?> 'PropertyType)
            | _ -> None
        let readPropertyAsString<'Result> = readPropertyAsType<'Result, string>
        let readPropertyAsDecimal<'Result> = readPropertyAsType<'Result, decimal>

        let bind index = 
            let entity = 
                {
                    index = index
                    properties = Map.empty
                    results = list.Empty
                }
            entity
            //Task.fromResult entity
        let bind32 index =
            bind (int64(index))

    module Transformer = 
        let empty<'Error> =
            fun entity -> Ok entity
        let defaultTransformer<'Result> (entity: PipelineProcessData<'Result>) = 
            task {
                return entity
            }
            

    type Pipeline<'Results> = 
        {
            extractor: Reader<'Results>
            transformers: Transformer<'Results> taskSeq
        }
    module Pipeline = 
        let bind reader transformers = 
            { 
                extractor = reader 
                transformers = transformers
            }

    let transformEntity transformers entity = 
        let fold state transformer= 
            backgroundTask {
                let! entity = transformer state
                return entity
            }
        backgroundTask {
            return!
                transformers
                |> TaskSeq.foldAsync fold entity 
        }

    let processPipeline pipelineDefinition =     
        pipelineDefinition.extractor ()
        |> TaskSeq.mapAsync (transformEntity pipelineDefinition.transformers)

    //let executePipeline<'Result> (pipelineStream: PipelineProcessData<'Result>  taskSeq)=
    //    taskSeq {
    //        return! pipelineStream
    //    }
        
