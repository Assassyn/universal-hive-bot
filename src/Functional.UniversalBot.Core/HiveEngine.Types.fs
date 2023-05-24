module HiveEngineTypes

open System.Net.Http
open HiveAPI
open System.Text.Json
open FsHttp

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
            balance = raw.balance |> Decimal.fromString |> Some.defaultWhenNone 0M
            stake = raw.stake |> Decimal.fromString |> Some.defaultWhenNone 0M
            pendingUnstake = raw.pendingUnstake |> Decimal.fromString |> Some.defaultWhenNone 0M
            delegationsIn = raw.delegationsIn |> Decimal.fromString |> Some.defaultWhenNone 0M
            delegationsOut = raw.delegationsOut |> Decimal.fromString |> Some.defaultWhenNone 0M
            pendingUndelegations = raw.pendingUndelegations |> Decimal.fromString |> Some.defaultWhenNone 0M
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
            quantity = raw.quantity |> Decimal.fromString |> Some.defaultWhenNone 0M
            price = raw.price |> Decimal.fromString |> Some.defaultWhenNone 0M
            priceDec = {|
                ``$numberDecimal`` = raw.priceDec.``$numberDecimal`` |> Decimal.fromString |> Some.defaultWhenNone 0M
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
            quantity = raw.quantity |> Decimal.fromString |> Some.defaultWhenNone 0M
            quantityLeft = raw.quantityLeft |> Decimal.fromString |> Some.defaultWhenNone 0M
            nextTransactionTimestamp = raw.nextTransactionTimestamp
            numberTransactionsLeft = raw.numberTransactionsLeft
        }


type RawLiqudityPools = 
   {
       //_id: int64
       precision: int32
       tokenPair: string
       baseQuantity: string
       baseVolume: obj
       basePrice: string
       quoteQuantity: string
       quoteVolume: obj
       quotePrice: string
       totalShares: string
       creator: string
   }
type LiqudityPools = 
    {
        precision: int32
        tokenPair: string
        baseQuantity: decimal
        baseVolume: decimal
        basePrice: decimal
        quoteQuantity: decimal
        quoteVolume: decimal
        quotePrice: decimal
        totalShares: decimal
        creator: string
    }
module LiqudityPools =
    let bind (raw: RawLiqudityPools) = 
        {
            precision = raw.precision 
            tokenPair = raw.tokenPair 
            baseQuantity = raw.baseQuantity |> Decimal.fromString |> Some.defaultWhenNone 0M
            baseVolume = raw.baseVolume.ToString() |> Decimal.fromString |> Some.defaultWhenNone 0M
            basePrice = raw.basePrice |> Decimal.fromString |> Some.defaultWhenNone 0M
            quoteQuantity = raw.quoteQuantity |> Decimal.fromString |> Some.defaultWhenNone 0M
            quoteVolume = raw.quoteVolume.ToString() |> Decimal.fromString |> Some.defaultWhenNone 0M
            quotePrice = raw.quotePrice |> Decimal.fromString |> Some.defaultWhenNone 0M
            totalShares = raw.totalShares |> Decimal.fromString |> Some.defaultWhenNone 0M
            creator = raw.creator 
        }

type RawTokenInfo =
    {
        _id: int32
        issuer: string
        symbol: string
        name: string
        precision: int32
        maxSupply: string
        supply: string
        circulatingSupply: string
        stakingEnabled: bool
        unstakingCooldown:  int32
        delegationEnabled: bool
        undelegationCooldown: int32
        numberTransactions: int32
        totalStaked: string
    }
type TokenInfo =
    {
        issuer: string
        symbol: string
        name: string
        precision: int32
        maxSupply: decimal
        supply: decimal
        circulatingSupply: decimal
        stakingEnabled: bool
        unstakingCooldown:  int32
        delegationEnabled: bool
        undelegationCooldown: int32
        numberTransactions: int32
        totalStaked: decimal
    }

module TokenInfo =
    let bind (raw: RawTokenInfo) = 
        {
            issuer = raw.issuer
            symbol = raw.symbol
            name = raw.name
            precision = raw.precision
            maxSupply = raw.maxSupply |> Decimal.fromString |> Some.defaultWhenNone 0M
            supply = raw.supply|> Decimal.fromString |> Some.defaultWhenNone 0M
            circulatingSupply = raw.circulatingSupply|> Decimal.fromString |> Some.defaultWhenNone 0M
            stakingEnabled = raw.stakingEnabled
            unstakingCooldown = raw.unstakingCooldown
            delegationEnabled = raw.delegationEnabled
            undelegationCooldown = raw.undelegationCooldown
            numberTransactions = raw.numberTransactions
            totalStaked = raw.totalStaked|> Decimal.fromString |> Some.defaultWhenNone 0M
        }
    let getTokenPrecision (tokens: TokenInfo seq) tokenSymbol =
        let token = 
            tokens
            |> Seq.find (fun x -> x.symbol = tokenSymbol)
        token.precision
