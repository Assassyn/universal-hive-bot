module ConfigurationTypes

open Types
open Functional.ETL.Pipeline

type Loger = string -> string -> string -> unit

type Binder = Loger -> Urls -> Map<string, string> -> Transformer<PipelineResult.UniversalHiveBotResutls>
        
type UserActionReader = UserActionsDefinition seq -> Reader<PipelineResult.UniversalHiveBotResutls>
