module LoggingService

open System

open GlobalTypes
open Model

let logRecords state = 
    match state with 
    Success _ -> Console.WriteLine "success"
    //| Success (sapCustomers:SAPCustomer list) -> sapCustomers |> List.map Console.WriteLine |> ignore 
    | Failure (s:string) -> Console.WriteLine s

