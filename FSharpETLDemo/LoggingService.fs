module LoggingService

open System

open GlobalTypes

let logRecords state = 
    match state with 
    | Success _ -> Console.WriteLine "success"
    | Failure (s:string) -> Console.WriteLine s

