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

let replaceString (old: string) replaceWith (input: string) =
    input.Replace(old, replaceWith)

let generateRandomString numebrOfCharacters = 
    let randomizer = Random()
    let chars = "0123456789abcdefghijklmnopqrstuvwxyz".ToCharArray()
    let sz = Array.length chars in
    String(Array.init numebrOfCharacters (fun _ -> chars.[randomizer.Next sz])).ToString()
