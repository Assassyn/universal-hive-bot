module Int16

open System

let fromString (input: string) =
    let mutable number = 0s
    if Int16.TryParse (input, &number)
    then Some number
    else None
