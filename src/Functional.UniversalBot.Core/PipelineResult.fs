module PipelineResult

type Module = string
type Item = string
type Message = string
type TransactionId = string
type CustomJson = HiveAPI.COperations.custom_json
type Token = string 
type Username = string
type KeyRequired =
    | Active
    | Posting

type UniversalHiveBotResutls =
    | Unknow
    | NoUserDetails of Module
    | TokenBalanceLoaded of Username
    | HiveOperation of Module * Token * KeyRequired * CustomJson
    | FlushingFinshed of Username * UniversalHiveBotResutls seq
    | TokenBalanceTooLow of Module * Username * Token 
    | Processed of Module * Item
    | UnableToProcess of Module * Item * Message
