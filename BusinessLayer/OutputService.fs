module OutputService

open System

open GlobalTypes
open Model

let saveCustomer (repoFunc) (customer:WCCustomer) = 
    try
        let result = repoFunc customer
        Success (Some result)
    with
        | :? Exception as ex -> (String.Format("Exception when saving customer{0}Message:{1}{0}StackTrace:{2}", Environment.NewLine, ex.Message, ex.StackTrace)) |> Failure //todo: reusable error message formatting
