module PipelineResult

type Module = string
type Item = string
type Message = string
type TransactionId = string
type HiveOperation = HiveAPI.COperations.IOperationID
type CustomJson = HiveAPI.COperations.custom_json
type Token = string 
type Username = string
type Action = string
type KeyRequired =
    | Active
    | Posting
    | ActiveSingle
    | PostingSingle

type UniversalHiveBotResutls =
    | Unknow
    | NoUserDetails of Module
    | TokenBalanceLoaded of Username
    | Loaded of Action
    | HiveOperation of Module * Token * KeyRequired * HiveOperation
    | FlushingFinshed of Username * UniversalHiveBotResutls seq
    | TokenBalanceTooLow of Module * Username * Token 
    | Processed of Module * Item
    | UnableToProcess of Module * Item * Message
