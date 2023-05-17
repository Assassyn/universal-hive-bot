module FunctionalString

open System

let asDecimal (input: string) =
    let mutable number = 0M
    if Decimal.TryParse  (input, &number)
    then number
    else 0M

let asString input = 
    input.ToString()

let asStringWithPrecision (input: decimal) = 
    input.ToString("F3")

