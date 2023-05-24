module ConfigurationTypes

open Types
open Functional.ETL.Pipeline

type Binder = Urls -> Map<string, string> -> Transformer<PipelineResult.UniversalHiveBotResutls>
        
type UserActionReader = UserActionsDefinition seq -> Reader<PipelineResult.UniversalHiveBotResutls>
