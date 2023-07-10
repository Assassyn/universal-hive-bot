module LoadTemplate

open System
open System.Threading
open PipelineResult
open Pipeline
open PipelineProcessData
open Nettle
open Nettle.Functions

type ReadEntityFunction (entity: PipelineProcessData<UniversalHiveBotResutls>) =
    inherit FunctionBase ()

    override this.Description =
        "Reads property from entity";
    override this.GenerateOutput (request, cancellationToken) = 
        task {  
            let key = request.ParameterValues.[0].ToString ()
            
            if entity.properties.ContainsKey (key) 
            then 
                return entity.properties.[key]
            else 
                return (String.Empty :> obj)
        }

type EnumerateEntityFunction (entity: PipelineProcessData<UniversalHiveBotResutls>) =
    inherit FunctionBase ()

    override this.Description =
        "Enumerates property from entity";
    override this.GenerateOutput (request, cancellationToken) = 
        task {  
            let key = request.ParameterValues.[0].ToString ()
            
            let items = enumerateProperties key entity |> Array.ofSeq
            return items
        }

type CompareDatesFunction () = 
    inherit FunctionBase ()

    override this.Description =
        "Enumerates property from entity";
    override this.GenerateOutput (request, cancellationToken) = 
        task {  
            let left = request.ParameterValues.[0].ToString () |> DateTime.Parse
            let comparer = request.ParameterValues.[1].ToString ()
            let right = request.ParameterValues.[2].ToString () |> DateTime.Parse

            return 
                match comparer with 
                | "=" -> left = right
                | ">=" -> left >= right
                | "<=" -> left <= right
                | ">" -> left > right
                | "<" -> left < right
                | _ -> false
        }

[<Literal>]
let private ModuleName = "LoadTemplate" 

let private replaceMustache entity (input: string) =
    task {
        let compiler 
            = NettleEngine.GetCompiler([|        
                new ReadEntityFunction (entity) :> Functions.IFunction
                new EnumerateEntityFunction (entity) :> Functions.IFunction
                new CompareDatesFunction() :> Functions.IFunction
            |])
        let execute = compiler.Compile(input)
        let! output = execute.Invoke(entity, CancellationToken.None)
        return output
    }

let action templateId label username (entity: PipelineProcessData<UniversalHiveBotResutls>) = 
    TemplateAPI.getTemplate templateId
    |> replaceMustache entity
    |> withProperty entity label 
    |>= Loaded (sprintf "Template with id %s" templateId)

let bind urls (parameters: Map<string, string>) = 
    let templateId = Map.getValueWithDefault parameters "templateId" ""
    let label = Map.getValueWithDefault parameters "label" "template"
    Action.bindAction ModuleName (action templateId label)

