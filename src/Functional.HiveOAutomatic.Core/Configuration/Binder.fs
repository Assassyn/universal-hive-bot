module Binder

open Microsoft.Extensions.Configuration

let getConfiguration () = 
    let config = new ConfigurationBuilder()

    let config = config.AddJsonFile ("users.json", true)
    let config = config.AddYamlFile ("users.yaml", true)
    let config = config.AddYamlFile ("users.yml", true)

    let config = config.AddJsonFile ("config.json", true)
    let config = config.AddYamlFile ("config.yaml", true)
    let config = config.AddYamlFile ("config.yml", true)

    config.Build()