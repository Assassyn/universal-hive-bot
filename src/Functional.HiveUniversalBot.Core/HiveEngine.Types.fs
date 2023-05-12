module HiveEngineTypes

open System.Net.Http
open HiveAPI
open System.Text.Json
open FsHttp
open FunctionalString

type HiveResponse<'Result> =
    {
        jsonrpc: string
        id: int64
        result: 'Result seq
    }
   
type RawTokenBalance = 
    {
        _id: int64
        account: string
        symbol: string
        balance: string
        stake: string
        pendingUnstake: string
        delegationsIn: string
        delegationsOut: string
        pendingUndelegations: string
    }
type TokenBalance = 
    {
        account: string
        symbol: string
        balance: decimal
        stake: decimal
        pendingUnstake: decimal
        delegationsIn: decimal
        delegationsOut: decimal
        pendingUndelegations: decimal
    }
module TokenBalance = 
    let bind (raw:RawTokenBalance) = 
        {
            account = raw.account
            symbol = raw.symbol
            balance = raw.balance |> asDecimal
            stake = raw.stake |> asDecimal
            pendingUnstake = raw.pendingUnstake |> asDecimal
            delegationsIn = raw.delegationsIn |> asDecimal
            delegationsOut = raw.delegationsOut |> asDecimal
            pendingUndelegations = raw.pendingUndelegations |> asDecimal
        }

type RawMarketBuyBook = 
    {
        _id: int64
        txId: string
        timestamp: int32
        account: string
        symbol: string
        quantity: string
        price: string
        priceDec: {|
            ``$numberDecimal``: string
        |}
        expiration: int64
    } 
type MarketBuyBook = 
    {
        txId: string
        timestamp: int32
        account: string
        symbol: string
        quantity: decimal
        price: decimal
        priceDec: {|
            ``$numberDecimal``: decimal
        |}
    }
module MarketBuyBook =
    let bind (raw: RawMarketBuyBook) = 
        {
            txId = raw.txId 
            timestamp = raw.timestamp 
            account = raw.account 
            symbol = raw.symbol 
            quantity = raw.quantity |> asDecimal
            price = raw.price |> asDecimal
            priceDec = {|
                ``$numberDecimal`` = raw.priceDec.``$numberDecimal`` |> asDecimal
            |}
        }

type RawPendingUnstakes = 
    {
        _id: int64
        account: string
        symbol: string
        quantity: string
        quantityLeft: string
        nextTransactionTimestamp: int64
        numberTransactionsLeft: int16
        millisecPerPeriod: string
        txID: string
    }
type PendingUnstakes = 
    {
        account: string
        symbol: string
        quantity: decimal
        quantityLeft: decimal
        nextTransactionTimestamp: int64
        numberTransactionsLeft: int16
    }
module PendingUnstakes =
    let bind (raw: RawPendingUnstakes) = 
        {
            account = raw.account
            symbol = raw.symbol
            quantity = raw.quantity |> asDecimal
            quantityLeft = raw.quantityLeft |> asDecimal
            nextTransactionTimestamp = raw.nextTransactionTimestamp
            numberTransactionsLeft = raw.numberTransactionsLeft
        }
