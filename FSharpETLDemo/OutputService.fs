module OutputService

open System

open GlobalTypes

let saveCustomers repoFunc state = 
    try
        let result = repoFunc()
        Success result
    with
        | :? Exception as ex -> "" |> Failure 
