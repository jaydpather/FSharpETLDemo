module StateChecker

open System

open GlobalTypes

let checkState nextFunction state = 
    match state with
    | Success s -> 
        match s with 
        | Some data -> nextFunction data
        | None _ -> Success None //success none means there were no records to load
    | Failure f -> Failure f

let checkEmptyState nextFunction state = 
    match state with
    | Success(None) -> state
    | x -> nextFunction x