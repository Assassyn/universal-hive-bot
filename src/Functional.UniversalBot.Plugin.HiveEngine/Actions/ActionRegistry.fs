module ActionRegistry_

open ConfigurationTypes
open Lamar
open Functional.ETL.Pipeline

type ActionRegistry () as self =
    inherit ServiceRegistry ()
    do 
        let defaultBinder = (fun url properties -> Transformer.defaultTransformer<PipelineResult.UniversalHiveBotResutls>)
        self.For<Binder>().Use(StakeToken.bind).Named("stake") |> ignore
        self.For<Binder>().Use(UnstakeToken.bind).Named("unstake") |> ignore
        self.For<Binder>().Use(DelegateStake.bind).Named("delegatestake") |> ignore
        self.For<Binder>().Use(UndelegateStake.bind).Named("undelegateStake") |> ignore
        self.For<Binder>().Use(Balance.bind).Named("balance") |> ignore
        self.For<Binder>().Use(SellToken.bind).Named("sell") |> ignore
        self.For<Binder>().Use(TransferToken.bind).Named("transfer") |> ignore
        self.For<Binder>().Use(AddTokenToPool.bind).Named("addtopool") |> ignore
        self.For<Binder>().Use(FlushAndBalanceAction.bind).Named("flushandbalance") |> ignore
        self.For<Binder>().Use(TokenSwapAction.bind).Named("swaptoken") |> ignore     
        self.For<Binder>().UseIfNone(defaultBinder) 
        self.For<UserActionReader>().Use(UserReader.bind) |> ignore
