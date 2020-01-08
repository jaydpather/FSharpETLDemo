module LoggingService

open System

open GlobalTypes
open Model

let logRecord state = 
    match state with 
    | Success _ -> Console.WriteLine "success"
    //| Success customerOption -> 
    //    match customerOption with 
    //        | Some (customer:WCCustomer) -> customer |> Console.WriteLine |> ignore 
    //        | None _ -> "no records" |> Console.WriteLine |> ignore
    | Failure (s:string) -> Console.WriteLine s

