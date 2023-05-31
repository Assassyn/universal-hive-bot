module Json

open System.Text.Json

let deserialize<'ResponsePayload> (json: JsonElement) = 
    JsonSerializer.Deserialize<'ResponsePayload> json
