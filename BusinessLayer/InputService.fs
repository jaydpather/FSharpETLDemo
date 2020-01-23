module InputService

open GlobalTypes
open Model
open System
open InputRepository
open InputRepositoryFactory

let updateImportStatus (inputRepoCtx:IInputRepositoryContext) state = 
    try
        inputRepoCtx.updateImportStatus state
    with
    | :? Exception as ex -> (String.Format("failed to update import status{0}Message:{1}{0}Stack Trace:{2}", Environment.NewLine, ex.Message, ex.StackTrace)) |> Failure //todo: NewFailure


//NOTE: allowing strings to be null instead of using string option
let loadCustomers loadCustomersRepoFunc =
    try
        let loadedCustomer = loadCustomersRepoFunc()
        match loadedCustomer with 
        | Some (customer:SAPCustomer) ->
            match customer.CompanyCode with 
            //todo: null and "" cases are not possible to hit right now, because the select query has JOINs that prevent this from happening
            | null //never seen null, but maybe this will change with later versions of SqlDataReader
            | "" -> Failure (String.Format("CustomerNumber {0} has no CompanyCode. (missing CustomerCompany record)", customer.CustomerNumber))
            | _ -> Success (Some customer)
        | None -> Success None //Success None means there were no records to load
    with
        | :? Exception as ex -> String.Format("Exception when trying to load input customers.{2}Message:{0},{2}Stack Trace:{1}", ex.StackTrace, ex.Message, Environment.NewLine) |> Failure 
