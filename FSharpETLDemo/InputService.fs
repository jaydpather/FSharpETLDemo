module InputService

open GlobalTypes
open Model
open System


let loadCustomers loadCustomersRepoFunc x =
    try
        loadCustomersRepoFunc()
    with
        | :? Exception as ex -> String.Format("Exception.{2}Message:{0},{2}Stack Trace:{1}", ex.StackTrace, ex.Message, Environment.NewLine) |> Failure 
