module String

open System

let asDecimal (input: string) =
    let mutable number = 0M
    if Decimal.TryParse  (input, &number)
    then number
    else 0M
