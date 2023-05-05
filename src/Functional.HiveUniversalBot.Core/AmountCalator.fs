module AmountCalator

open System

let private ZeroWhenNegative value = 
    if value < 0M
    then
        0M
    else
        value

let private (|StaticValue|_|) (str: string) =
    let valueWhenLargerThanLimit limit value = 
        if limit >= value 
        then 
            value
        else
            limit
    let mutable limit = 0M
    if Decimal.TryParse(str, &limit) then Some(valueWhenLargerThanLimit limit)
    else None

let private (|Calculation|_|) (str: string) =
    if str.Contains ('-') then
        let numberOfTokensToIgnore = 
            let tokens = str.Split ('-')
            tokens.[1]
        let mutable value = 0M
        if Decimal.TryParse(numberOfTokensToIgnore, &value) then Some(fun x -> x - value)
        else None
    else None

let bind (value: string) =
    let calculation = 
        match value with 
        | StaticValue x -> x
        | Calculation x -> x
        | "*" | _  -> (fun amount -> amount)

    calculation >> ZeroWhenNegative
