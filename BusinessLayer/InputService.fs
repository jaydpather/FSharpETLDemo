module InputService

open GlobalTypes
open Model
open System
open InputRepository
open InputRepositoryFactory

let updateImportStatus (inputRepoCtx:IInputRepositoryContext) state = 
    let createRowCountMsg (rowCount:int) = 
        String.Format("Expected 1 row affected, but {0} rows were affected.", rowCount)

    let appendRowCountMsg msg rowCount = 
        String.Format("{0}{1}{2}", msg, Environment.NewLine, createRowCountMsg rowCount);

    let createDeleteFailure (successInfo:SuccessInfo) msg  = NewFailure { 
            Message = msg;
            InputStatusUpdateInfo = {
                CustomerNumber = successInfo.CustomerNumber;
                CompanyCode = successInfo.CompanyCode;
                NextImportStatus = "Failed"; //todo: remove this dummy value
            }
        }

    let createUpdateFailure (failureInfo:FailureInfo) msg = NewFailure {
        failureInfo with
            Message = msg
        }

    try
        let (rowCount, createFailureFunc) =  
            match state with 
            |NewSuccess(successInfo) -> 
                ( //todo: updgrade to VS2019 so you can use anonymous records. (this is a bug that MS won't fix for VS2017)
                    inputRepoCtx.deleteSuccessfulRecord successInfo,
                    fun rowCount -> 
                        appendRowCountMsg "Customer saved in WC successfully, but failed to delete input record." rowCount
                        |> createDeleteFailure successInfo  
                ) 
            |NewFailure failureInfo -> 
                (
                    inputRepoCtx.updateFailedRecordStatus failureInfo,
                    fun rowCount -> 
                        appendRowCountMsg "Failed to update ImportStatus of input record" rowCount
                        |> createUpdateFailure failureInfo 
                )
            //should never hit cases for Failure and Success. //todo: fix compiler warning for missing cases

        match rowCount with 
        |1 -> state
        |_ -> createFailureFunc rowCount

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
