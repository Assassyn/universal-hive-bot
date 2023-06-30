﻿module Variable

open PipelineResult
open Pipeline
open Types
open PipelineProcessData
open BridgeAPITypes
open System


let action name value (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    task {
        return 
            withProperty entity name value 
            |>= Loaded $"Variable {name}"
    }

let bind urls (parameters: Map<string, string>) = 
    let name = Map.getValueWithDefault parameters "name" ""
    let value = Map.getValueWithDefault parameters "value" ""
    action name value 
