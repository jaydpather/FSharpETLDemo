module InputService

open GlobalTypes
open Model
open System


let loadCustomers loadCustomersRepoFunc =
    try
        loadCustomersRepoFunc()
    with
        | :? Exception as ex -> String.Format("Exception when trying to load input customers.{2}Message:{0},{2}Stack Trace:{1}", ex.StackTrace, ex.Message, Environment.NewLine) |> Failure 
