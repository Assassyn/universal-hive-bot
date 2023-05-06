module PipelineResult

type Module = string
type Item = string
type Message = string
type CustomJson = HiveAPI.COperations.custom_json
type Token = string 
type KeyRequired =
    | Active
    | Posting

type UniversalHiveBotResutls =
    | Unknow
    | NoUserDetails of Module
    //| (*HiveOperation*) of Module * Token * KeyRequired * CustomJson
    | TokenBalanceTooLow of Module * Token 
    | Processed of Module * Item 
    | UnableToProcess of Module * Item * Message
