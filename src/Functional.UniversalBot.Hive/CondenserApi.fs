module CondenserApi

open System.Net.Http
open HttpQuery
open HiveTypes

let getDynamicGlobalroperties hiveUrl = 
    runHiveQuery<Properties> hiveUrl "condenser_api.get_dynamic_global_properties" emptyParameters

let getHeadBlock hiveUrl = 
    let globalProperties = 
        getDynamicGlobalroperties hiveUrl

    globalProperties.head_block_number
