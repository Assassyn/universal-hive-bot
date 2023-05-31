module HiveActionRegistry

open ConfigurationTypes
open Lamar
open Functional.ETL.Pipeline

type ActionRegistry () as self =
    inherit ServiceRegistry ()
    do 
        self.For<Binder>().Use(FlushTokens.bind).Named("flush") |> ignore
