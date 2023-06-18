module Configuration

open Microsoft.Extensions.Configuration
open Types

let getConfiguration () = 
    let config = 
        let config = new ConfigurationBuilder()
        let config = config.AddJsonFile ("configuration.json", true)
        let config = config.AddYamlFile ("configuration.yaml", true)
        let config = config.AddYamlFile ("configuration.yml", true)
        let config = config.AddUserSecrets ("01fcd8b6-1786-4a55-b2e8-6b24e7efaea8", true)
        config.Build()

    let actions = config.GetSection("actions").Get<UserActionsDefinition array> ()
    let urls = config.GetSection("urls").Get<Urls>()
    {
        urls = urls
        actions = actions
    }
