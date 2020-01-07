module LoggingService

open System

open Model
open GlobalTypes

//let logRecords (records:SAPCustomer list) = 
//    records |> List.map Console.WriteLine

let logRecords state = 
    match state with 
    | Success _ -> ()
    | Failure (records:SAPCustomer list) -> records |> List.map Console.WriteLine |> ignore

