module OutputService

open System

open GlobalTypes
open Model

let generateFailureInfo (ex:Exception) customer = 
    match ex.Message.Contains("Violation of PRIMARY KEY constraint 'PK_dbo_Customers'") with 
    | true -> ({
        Message = LoggingService.formatExceptionMessage "duplicate primary key - record will be retried" ex;
        InputStatusUpdateInfo = Some ({
            CustomerNumber = customer.CustomerNumber;
            CompanyCode = customer.CompanyCode;
            NextImportStatus = "New"; //todo: reference constant
        })
    })
    | false -> ({
        Message = LoggingService.formatExceptionMessage "unkown error when saving WC Customer" ex;
        InputStatusUpdateInfo = Some ({
            CustomerNumber = customer.CustomerNumber;
            CompanyCode = customer.CompanyCode;
            NextImportStatus = "Failed"; //todo: reference constant
        })
    })

let saveCustomer (repoFunc) (customer:WCCustomer) = 
    try
        let result = repoFunc customer
        Success (Some result)
    with
        | :? Exception as ex -> generateFailureInfo ex customer |> NewFailure //todo: reusable error message formatting

