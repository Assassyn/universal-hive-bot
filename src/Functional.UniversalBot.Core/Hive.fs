module Hive

open PipelineResult

let scheduleActiveOperation moduleName tokenSymbol operation = 
    HiveOperation (moduleName, tokenSymbol, KeyRequired.Active, operation)

let schedulePostingOperation moduleName tokenSymbol operation = 
    HiveOperation (moduleName, tokenSymbol, KeyRequired.Posting, operation)

let scheduleSinglePostingOperation moduleName tokenSymbol operation = 
    HiveOperation (moduleName, tokenSymbol, KeyRequired.PostingSingle, operation)
