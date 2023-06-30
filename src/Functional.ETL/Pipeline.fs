module Pipeline

    open FSharp.Control
    open System.Threading.Tasks

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
    type Transformers<'Result> = Transformer<'Result> taskSeq

    module Transformer = 
        let empty<'Error> =
            fun entity -> Ok entity
        let defaultTransformer<'Result> (entity: PipelineProcessData<'Result>) = 
            task {
                return entity
            }
            
    type Pipeline<'Results> = 
        {
            name: string
            settings: Map<string, string>
            extractor: Reader<'Results>
            transformers: Transformer<'Results> taskSeq
        }
    module Pipeline = 
        let bindCustom name settings reader transformers = 
            { 
                name = name
                settings = settings
                extractor = reader 
                transformers = transformers
            }
        let bind reader transformers = 
            bindCustom (System.Guid.NewGuid().ToString()) (Map.empty) reader transformers

    let transformEntity<'Entity> (transformers: Transformers<'Entity>) (entity: PipelineProcessData<'Entity>): Task<PipelineProcessData<'Entity>> = 
        let fold state transformer= 
            task {
                let! entity = transformer state
                return entity
            }
        task {
            return! 
                transformers
                |> TaskSeq.foldAsync fold entity 
        }

    let processPipeline pipelineDefinition =
        pipelineDefinition.extractor ()
        |> TaskSeq.mapAsync (transformEntity pipelineDefinition.transformers)
        |> TaskSeq.toArray
        
