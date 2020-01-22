module InputRepositoryFactory

open Model
open GlobalTypes

type IInputRepositoryContext = 
    abstract member loadCustomer : unit -> SAPCustomer option;
    abstract member updateImportStatus  : Result<string option, SuccessInfo, string, FailureInfo> -> Result<string option, SuccessInfo, string, FailureInfo>

//todo: why do i get a compile error when i move InputRepositoryContext into InputRepository.fs?  
type InputRepositoryContext = 
    { 
        loadCustomer : unit -> SAPCustomer option;
        updateImportStatus : Result<string option, SuccessInfo, string, FailureInfo> -> Result<string option, SuccessInfo, string, FailureInfo> 
    } 
    interface IInputRepositoryContext with
        member this.loadCustomer() = 
            this.loadCustomer ()
        member this.updateImportStatus(state) = 
            this.updateImportStatus state


let getInputRepositoryContext connectionString = 
    //todo: can this method return an anonymous record that implements the interface?
    //  (then you wouldn't need the record type InputRepositoryContext, which is a lot of extra typing)
    {
        loadCustomer = fun () -> 
            SQLUtils.getExecuteReaderFunc connectionString
            |> InputRepository.loadCustomers 

        updateImportStatus = fun result -> 
            SQLUtils.getSqlCmdFunc connectionString
            |> InputRepository.updateInputStatus result  
    }
    :> IInputRepositoryContext