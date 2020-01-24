module InputRepositoryFactory

open Model
open GlobalTypes
open InputRepository

type IInputRepositoryContext = 
    abstract member loadCustomer : unit -> SAPCustomer option;
    abstract member updateFailedRecordStatus : FailureInfo -> int
    abstract member deleteSuccessfulRecord : SuccessInfo -> int

//todo*: why do i get a compile error when i move InputRepositoryContext into InputRepository.fs?  
type InputRepositoryContext = 
    { 
        loadCustomer : unit -> SAPCustomer option;
        updateFailedRecordStatus : FailureInfo -> int
        deleteSuccessfulRecord : SuccessInfo -> int
    } 
    interface IInputRepositoryContext with
        member this.loadCustomer() = 
            this.loadCustomer ()
        member this.updateFailedRecordStatus(failureInfo) = 
            this.updateFailedRecordStatus failureInfo
        member this.deleteSuccessfulRecord(successInfo) = 
            this.deleteSuccessfulRecord successInfo


let getInputRepositoryContext connectionString = 
    {
        loadCustomer = fun () -> 
            SQLUtils.getExecuteReaderFunc connectionString
            |> InputRepository.loadCustomers;

        updateFailedRecordStatus = fun failureInfo ->
            SQLUtils.getSqlCmdFunc connectionString
            |> InputRepository.updateFailedRecord failureInfo;

        deleteSuccessfulRecord = fun successInfo ->
            SQLUtils.getSqlCmdFunc connectionString 
            |> InputRepository.deleteSuccessfulRecord successInfo
    }
    :> IInputRepositoryContext