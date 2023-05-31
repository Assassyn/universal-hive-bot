module TerracoreActionRegistry

open ConfigurationTypes
open Lamar
open Functional.ETL.Pipeline

type ActionRegistry () as self =
    inherit ServiceRegistry ()
    do 
        self.For<Binder>().Use(TerracoreClaim.bind).Named("terracore_claim") |> ignore
        self.For<Binder>().Use(TerracoreBalance.bind).Named("terracore_balance") |> ignore
