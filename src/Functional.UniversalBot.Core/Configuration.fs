module Configuration

open Microsoft.Extensions.Configuration
open Types

let getConfiguration (config: IConfiguration) = 
    let actions = config.GetSection("actions").Get<UserActionsDefinition array> ()
    let urls = config.GetSection("urls").Get<Urls>()
    {
        urls = urls
        actions = actions
    }
