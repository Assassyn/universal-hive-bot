module PipelineResult

type Module = string
type Item = string
type Message = string

type UniversalHiveBotResutls =
    | Unknow
    | NoUserDetails of Module
    | Processed of Module * Item 
    | UnableToProcess of Module * Item * Message
