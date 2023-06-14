module Int16

open System

let fromString (input: string) =
    let mutable value = 0s
    if Int16.TryParse (input, &value)
    then Some value
    else None
