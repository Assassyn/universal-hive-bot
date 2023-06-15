module Bool

open System

let fromString (input: string) =
    let mutable value = false
    if Boolean.TryParse (input, &value)
    then Some value
    else None
