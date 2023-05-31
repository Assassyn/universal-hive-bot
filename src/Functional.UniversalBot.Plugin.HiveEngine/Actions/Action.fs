module Action

open PipelineResult

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
