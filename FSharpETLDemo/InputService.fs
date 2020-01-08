module InputService

open GlobalTypes
open Model
open System


let loadCustomers loadCustomersRepoFunc =
    try
        let loadedCustomer = loadCustomersRepoFunc()
        match loadedCustomer with 
        | Some (customer:SAPCustomer) ->
            match customer.CompanyCode with 
            | null //never seen null, but maybe this will change with later versions of SqlDataReader
            | "" -> Failure (String.Format("CustomerNumber {0} has no CompanyCode. (missing CustomerCompany record)", customer.CustomerNumber))
            | _ -> Success (Some customer)
        | None -> Success None //Success None means there were no records to load
    with
        | :? Exception as ex -> String.Format("Exception when trying to load input customers.{2}Message:{0},{2}Stack Trace:{1}", ex.StackTrace, ex.Message, Environment.NewLine) |> Failure 
