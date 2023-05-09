module Map

open System

let getValueWithDefault (parameters: Map<string, string>) key defaultValue =
    match parameters.ContainsKey (key) with 
    | true -> parameters.[key]
    | _ -> defaultValue
