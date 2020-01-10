module LoggingService

open System

open GlobalTypes
open Model

let logRecord state = 
    match state with 
    | Success (msgOption) ->
        match msgOption with 
        | Some (msg:string) -> Console.WriteLine msg
        | None -> ignore() //this means there were no records to select
    //| Success customerOption -> 
    //    match customerOption with 
    //        | Some (customer:WCCustomer) -> customer |> Console.WriteLine |> ignore 
    //        | None _ -> "no records" |> Console.WriteLine |> ignore
    | Failure (msg:string) -> Console.WriteLine msg

