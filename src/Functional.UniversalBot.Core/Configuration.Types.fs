module ConfigurationTypes

open Types
open Functional.ETL.Pipeline

type Binder = Urls -> Map<string, string> -> Transformer<PipelineResult.UniversalHiveBotResutls>
        
type PipelineReader = UserActionsDefinition seq -> Reader<PipelineResult.UniversalHiveBotResutls>

