module OutputService

open System

open GlobalTypes
open Model
open OutputRepositoryFactory

let generateFailureInfo (ex:Exception) customer = 
    //todo: data layer should actually check for PK violation and return a strongly typed failure
    match ex.Message.Contains("Violation of PRIMARY KEY constraint 'PK_dbo_Customers'") with 
    | true -> ({
        Message = LoggingService.formatExceptionMessage "duplicate primary key - record will be retried" ex;
        InputStatusUpdateInfo = {
            CustomerNumber = customer.CustomerNumber;
            CompanyCode = customer.CompanyCode;
            NextImportStatus = Constants.ImportStatusNames.New;
        }
    })
    | false -> ({
        Message = LoggingService.formatExceptionMessage "unkown error when saving WC Customer" ex;
        InputStatusUpdateInfo = {
            CustomerNumber = customer.CustomerNumber;
            CompanyCode = customer.CompanyCode;
            NextImportStatus = Constants.ImportStatusNames.Failed;
        }
    })

let saveCustomer outputRepoCtx (customer:WCCustomer) = 
    try
        let result = outputRepoCtx.saveCustomer customer
        NewSuccess result
    with
        | :? Exception as ex -> generateFailureInfo ex customer |> NewFailure //todo: replace type test with pattern shown below (in comment)
        //todo: strongly-typed failures for duplicate PK VS unknown error (so you can UT for these)
        //|ex -> generateFailureInfo ex customer |> NewFailure //todo: reusable error message formatting
