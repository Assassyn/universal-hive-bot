module Tests

open System
open Xunit

[<Fact>]
let ``Can read HIVE levle 2 tokens`` () =
    let wallet = HiveWallet.connect ()
