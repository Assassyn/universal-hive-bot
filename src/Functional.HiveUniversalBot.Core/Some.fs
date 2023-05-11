module Some

open System

let defaultWhenNone defaultValue opt =
    match opt with 
    | Some x -> x
    | _ -> defaultValue 
