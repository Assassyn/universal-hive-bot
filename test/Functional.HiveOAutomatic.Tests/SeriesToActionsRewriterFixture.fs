module SeriesToActionsRewriterFixture

open Xunit
open FsUnit.Xunit
open Types
open Configuration
open SeriesToActionsRewriter
open System.Collections.Generic

[<Fact>]
let ``Can rewrite series action into separate task`` () =
    let actionDefinition = new ActionDefinition ()
    actionDefinition.Name <- "series"
    actionDefinition.Parameters <- new Dictionary<string, string> ()
    actionDefinition.Parameters.Add("action", "Stake")
    actionDefinition.Parameters.Add("splitParameterName", "token")
    actionDefinition.Parameters.Add("splitOn", "ONEUP;CENT;PGM;ALIVE;NEOXAG;PIMP;COM")
    actionDefinition.Parameters.Add("amount", "*")

    splitToActualActionConfigurationItems actionDefinition
    |> Seq.length
    |> should equal 7
