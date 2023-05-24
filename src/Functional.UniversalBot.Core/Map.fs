module Map

open System
open System.Collections.Generic

let getValueWithDefault (parameters: Map<string, string>) key defaultValue =
    match parameters.ContainsKey (key) with 
    | true -> parameters.[key]
    | _ -> defaultValue

let fromDictionary<'Key, 'Value when 'Key : comparison> (dic: IDictionary<'Key, 'Value>): Map<'Key, 'Value> = 
    dic |> Seq.map (|KeyValue|) |> Map.ofSeq
