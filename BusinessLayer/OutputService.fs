module OutputService

open System

open GlobalTypes
open Model
open OutputRepositoryFactory

let generateFailureInfo (ex:Exception) customer = 
    match ex.Message.Contains("Violation of PRIMARY KEY constraint 'PK_dbo_Customers'") with 
    | true -> ({
        Message = LoggingService.formatExceptionMessage "duplicate primary key - record will be retried" ex;
        InputStatusUpdateInfo = {
            CustomerNumber = customer.CustomerNumber;
            CompanyCode = customer.CompanyCode;
            NextImportStatus = "New"; //todo: reference constant
        }
    })
    | false -> ({
        Message = LoggingService.formatExceptionMessage "unkown error when saving WC Customer" ex;
        InputStatusUpdateInfo = {
            CustomerNumber = customer.CustomerNumber;
            CompanyCode = customer.CompanyCode;
            NextImportStatus = "Failed"; //todo: reference constant
        }
    })

let saveCustomer outputRepoCtx (customer:WCCustomer) = 
    try
        let result = outputRepoCtx.saveCustomer customer
        Success (Some result)
    with
        | :? Exception as ex -> generateFailureInfo ex customer |> NewFailure //todo: reusable error message formatting

