module LoadTemplateFixture


open System
open Xunit
open FsUnit.Xunit
open Pipeline
open FSharp.Control
open Nettle
open LoadTemplate

let template = """Daily Report for 
# tl;dr
This is a daily summary of all post which has been supported by dcrop Boost 
# Voted on posts{{ var posts = @EnumerateEntity("post") }}{{ var today = @GetDate() }}{{ var yesterday = @AddDays(today, -1.0) }}
{{each posts}}{{var isIn24Hours = @CompareDates(yesterday, "<", $.date) }}{{ if isIn24Hours }}* {{$.name}} - {{isIn24Hours}}{{/if}}
{{/each}}"""

[<Fact>]
let ``Load more advanced template from entity`` () = 
    task { 
        let model = 
            PipelineProcessData.bind 1
            |> PipelineProcessData.addProperty "date" (DateTime.Now.ToShortDateString())
            |> PipelineProcessData.addProperty "post_001" {| postId = 1; name = "1"; date = DateTimeOffset.Now|}
            |> PipelineProcessData.addProperty "post_002" {| postId = 1; name = "2"; date = DateTimeOffset.Now|}
            |> PipelineProcessData.addProperty "post_003" {| postId = 1; name = "3"; date = DateTimeOffset.Now.AddDays(-2)|}
            |> PipelineProcessData.addProperty "post_004" {| postId = 1; name = "4"; date = DateTimeOffset.Now.AddDays(-2)|}

        let compiler 
            = NettleEngine.GetCompiler ([|
                new ReadEntityFunction (model) :> Functions.IFunction
                new EnumerateEntityFunction (model) :> Functions.IFunction
                new CompareDatesFunction() :> Functions.IFunction
            |])
        let execute = compiler.Compile (template)
        let! output = execute.Invoke (model.properties, System.Threading.CancellationToken.None)
        output |> should startWith """Daily Report for 
# tl;dr
This is a daily summary of all post which has been supported by dcrop Boost 
# Voted on posts
* 1 - True
* 2 - True"""
    }


[<Fact>]
let ``Load simple template from entity`` () = 
    task { 
        let simpleTemplate = """This post has been supported by @dcropsboost with {{@ReadEntity("weight")}}% upvote! Delegate HP to dCropsBoost to help support the project. 
        
        This project is not associated with dCrops developer but it would love to help promote the community. 
        
        [dCrops](https://dcrops.com/game)"""

        let model = 
            PipelineProcessData.bind 1
            |> PipelineProcessData.addProperty "date" (DateTime.Now.ToShortDateString())
            |> PipelineProcessData.addProperty "weight" "10.00"

        let compiler 
            = NettleEngine.GetCompiler([|
                new ReadEntityFunction (model) :> Functions.IFunction
                new EnumerateEntityFunction (model) :> Functions.IFunction
            |])
        let execute = compiler.Compile(simpleTemplate)
        let! output = execute.Invoke(model.properties, System.Threading.CancellationToken.None)
        output |> should startWith "This post has been supported by @dcropsboost with 10.00% upvote! Delegate HP to dCropsBoost to help support the project. "
    }
