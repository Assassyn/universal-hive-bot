# What is the Universal Hive Bot

This is a solution to my problems. I have a few tokesn which I need to stake, delegate stake, or sell. This processes are long and anoying so I need a small bot to do this taks for me.

Any question, advices or idea please create the issue.

# Why do I need to specyfy ther Active Key? 

Because bot is doing token operation it requires the ActiveKey to be able to authorise. I have added the posting key as to future-proof it just in case I will find an operation which can be automated and requires the posting authorization.

# But it can be better..

I am aware that there are a few places where I cna improve the bot. For example, I am sure if Balance and Flush action should be explicity declared or not. 

# Example configuration

``` JSON
{
  "urls": {
    "hiveNodeUrl": "https://anyx.io",
    "hiveEngineNodeUrl": "http://engine.alamut.uk:5000"
  },
  "actions": [
    {
      "username": "assassyn",
      "activeKey": "",
      "postingKey": "",
      "tasks": [
        {
          "name": "Balance"
        },
        {
          "name": "Stake",
          "parameters": {
            "token": "ONEUP",
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
          "name": "DelegateStake",
          "parameters": {
            "token": "PGM",
            "amount": "* - 50",
            "reciever": "lolz.pgm"
          }
        }
      ]
    }
  ]
}
```

Avaible actions: 
 * Balance -> reading all the lelve 2 tokens balance and staked balance
 * Stake -> moves desired amount to stake.

# Amount Calculation 

To allow slightly more advanced aproach to amont which have to be transfer there are 3 possible setting for it:
 * fixed amount like 100, it will transfer up to selected amount only
 * * -> it will transfer all availble tokesn 
 * * - 10 -> it will deduct 10 tokens from the availble amoutn and transfer the rest.

# How to support me

In a case you think that my work is useful and you want to help me, please consider supporting my Hive(https://vote.hive.uno/@assassyn) and HiveEngine witnesses (https://votify.vercel.app/alamut-he)
