module Decimal

open System

let roundToPrecision (precision: int) (number: decimal) = 
    System.Math.Round(number, precision)
