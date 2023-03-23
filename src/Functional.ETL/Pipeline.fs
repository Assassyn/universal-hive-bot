namespace Functional.ETL

module Pipeline =
    type Entity = 
        {
            index: int64
            properties: Map<string, obj>
        }
    module Entity = 
        let withProperty entity key value =
            let properties = entity.properties.Add (key, value)
            { entity with properties = properties }
        let readProperty entity key =
            entity.properties.[key]
        let bind index = 
            {
                index = index
                properties = Map.empty
            }
    type WriterResult = 
        | Success
        | Failure

    type ErrorMessage = string

    type Reader = unit -> Entity seq

    type Transformer<'Error> = Entity -> Result<Entity, 'Error>
    module Transformer = 
        let empty<'Error> =
            fun entity -> Ok entity
        let wrap<'Error> (transformFunction: Entity -> Entity): Transformer<'Error> = 
            transformFunction >> Ok


    type Pipeline<'Error> = 
        {
            extractor: Reader
            transformers: Transformer<'Error>
        }
    module Pipeline = 
        let bind reader transformers = 
            {
                extractor = reader 
                transformers = transformers
            }


    let process pipeline =     
        pipeline.extractor ()
        |> Seq.map pipeline.transformers