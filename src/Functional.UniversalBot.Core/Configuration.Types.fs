module ConfigurationTypes

open Types
open Pipeline

type Binder = Urls -> Map<string, string> -> Transformer<PipelineResult.UniversalHiveBotResutls>
        
module Binder = 
    let defaulBinder url properties =
        Transformer.defaultTransformer

type PipelineReader = UserActionsDefinition seq -> Reader<PipelineResult.UniversalHiveBotResutls>

