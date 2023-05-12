# What is the Universal Hive Bot

This is a solution to my problems. I have a few tokesn which I need to stake, delegate stake, or sell. This processes are long and anoying so I need a small bot to do this taks for me.

Any question, advices or idea please create the issue.

# Why do I need to specyfy ther Active Key? 

Because bot is doing token operation it requires the ActiveKey to be able to authorise. I have added the posting key as to future-proof it just in case I will find an operation which can be automated and requires the posting authorization.

# But it can be better..

I am aware that there are a few places where I cna improve the bot. For example, I am sure if Balance and Flush action should be explicity declared or not. 

# List of available tasks

 1. Balance - get the lsit of available level 2 tokens
 ```
{
    "name": "Balance" 
}
```

 2. Flush - gathered all scheduled operations and passes them to HIVE Blockchain. That way the bot can control the maximum number of operations per block. 
 ```
{
    "name": "Flush"
}
 ```

  3. Transfer - allows to transfer tokens to another acount 

  Parameters:
    * token - the name of the token
    * amount - the computable desired amount, check #AmountCalculation to see what can be used
    * transferTo - account to which token ahas to be transfer 
```
{
    "name": "Transfer",
    "parameters": {
        "token": "BEE",
        "amount": "*",
        "transferTo": "alamut-he"
    }
}
```

 4. Stake - transfer tokesn to stake

 Parameters:
    * token - the name of the token
    * amount - the computable desired amount, check #AmountCalculation to see what can be used
```
{
    "name": "Stake",
    "parameters": {
    "token": "ONEUP",
    "amount": "*"
    }
}
```

 5. Unstake - transfer tokens out of stake 

 Parameters:
    * token - the name of the token
    * amount - the computable desired amount, check #AmountCalculation to see what can be used
```
{
    "name": "Unstake",
    "parameters": {
        "token": "BEE",
        "amount": "*"
}
```

 6. DelegateStake - delegates staked tokens 

 Parameters:
    * token - the name of the token
    * amount - the computable desired amount, check #AmountCalculation to see what can be used
    * delegateTo - accoutn to which tokens will be delegated 
```
{
    "name": "DelegateStake",
    "parameters": {
        "token": "PGM",
        "amount": "* - 50",
        "delegateTo": "lolz.pgm"
    }
}
```

 7. UndelegateStake - undelegates token 

 Parameters:
    * token - the name of the token
    * amount - the computable desired amount, check #AmountCalculation to see what can be used
    * undelegateFrom - account from which tokens will be undelegated 

```
{
    "name": "UndelegateStake",
    "parameters": {
    "token": "PGM",
    "amount": "* - 50",
    "undelegateFrom": "lolz.pgm"
    }
}
```

 8. Sell - opening the order on the market, the query is simple and is trying to find the fist amount larger than the requested one

 Parameters:
    * token - the name of the token
    * amount - the computable desired amount, check #AmountCalculation to see what can be used

```
{
    "name": "Sell",
    "parameters": {
    "token": "WOO",
    "amountToSell": "*"
    }
}
```

# Example configuration

``` JSON
{
  "urls": {
    "hiveNodeUrl": "https://anyx.io",
    "hiveEngineNodeUrl": "http://engine.alamut.uk:5000"
  },
  "actions": [
    {
      "username": "universal-bot",
      "activeKey": "<activeKey here>",
      "postingKey": "<postingKey here>",
      "tasks": [
        {
          "name": "Balance" 
        },
        {
          "name": "Unstake",
          "parameters": {
            "token": "BEE",
            "amount": "*"
          }
        },
        {
          "name": "Flush"
        },
        {
          "name": "Balance"
        },
        {
          "name": "Sell",
          "parameters": {
            "token": "BEE",
            "amountToSell": "*"
          }
        },
        {
          "name": "Flush"
        }
      ]
    }
  ]
}
```

# Amount Calculation 

To allow slightly more advanced aproach to amont which have to be transfer there are 3 possible setting for it:
 * fixed amount like 100, it will transfer up to selected amount only
 * * -> it will transfer all availble tokesn 
 * * - 10 -> it will deduct 10 tokens from the availble amoutn and transfer the rest.

# How to support me

In a case you think that my work is useful and you want to help me, please consider supporting my Hive(https://vote.hive.uno/@assassyn) and HiveEngine witnesses (https://votify.vercel.app/alamut-he)
