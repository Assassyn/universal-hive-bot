module Decimal

open System

let roundToPrecision (precision: int) (number: decimal) = 
    System.Math.Round(number, precision)
    
let fromString (input: string) =
    let mutable number = 0M
    if Decimal.TryParse (input, &number)
    then Some number
    else None
