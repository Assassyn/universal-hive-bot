module String

open System

let asString input = 
    input.ToString()

let asStringWithPrecision (input: decimal) = 
    input.ToString("F5")

let split (character: string) (input: string) =
    input.Split(character)

let replace (old: Char) replaceWith (input: string) =
    input.Replace(old, replaceWith)
