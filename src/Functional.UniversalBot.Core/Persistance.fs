module Persistance

open Hanssens.Net

let private storage = new LocalStorage ()

let save key value =
    storage.Store(key, value)

let access<'ReturnType> key =
    storage.Get<'ReturnType>(key)
