module Int32

open System

let fromString (input: string) =
    let mutable number = 0
    if Int32.TryParse (input, &number)
    then Some number
    else None

let (++) number = 
    number + 1
