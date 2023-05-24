module Action

open PipelineResult

let buildCustomJson username method payload = 
    let json = System.Text.Json.JsonSerializer.Serialize (payload)
    Hive.createCustomJsonActiveKey username method json

let scheduleActiveOperation logger moduleName tokenSymbol operation = 
    logger tokenSymbol
    HiveOperation (moduleName, tokenSymbol, KeyRequired.Active, operation)

type CustomJsonMessage<'Payload> = 
    {
        contractName: string
        contractAction:string 
        contractPayload:'Payload
    }
let bindCustomJson<'Payload> contractName contractAction payload: CustomJsonMessage<'Payload> = 
    {
        contractName = contractName
        contractAction = contractAction
        contractPayload = payload
    }
