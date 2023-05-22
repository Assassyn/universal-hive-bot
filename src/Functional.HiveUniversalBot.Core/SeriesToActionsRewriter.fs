module SeriesToActionsRewriter

open Types

[<Literal>]
let private ModuleName = "series"

let private createSingleActionDefinition (parameters: Map<string, string>) = 
    let definition = new ActionDefinition ()

    definition.Name <- parameters["action"]
    definition.Parameters <- parameters

    [ definition ]

let private convertSeriesTaskToMultipleTasks (config: ActionDefinition) = 
    let parameters = config.Parameters |> Map.fromDictionary
    let splitOnParameterName = parameters["splitParameterName"]

    parameters["splitOn"]
    |> String.replace ',' ';'         
    |> String.split ";"
    |> Seq.map (fun parameter -> 
        parameters.Add (splitOnParameterName, parameter))
    |> Seq.collect createSingleActionDefinition

let splitToActualActionConfigurationItems (config: ActionDefinition): ActionDefinition seq =
    match config.Name.ToLower () with 
    | ModuleName -> 
        convertSeriesTaskToMultipleTasks config
    | _ -> 
        [ config ]
