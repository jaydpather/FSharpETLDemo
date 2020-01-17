module InputRepositoryFactory

open GlobalTypes
open Model

type InputRepositoryContext = { 
    loadCustomer : unit -> SAPCustomer option;
    updateImportStatus : Result<string option, SuccessInfo option, string, FailureInfo> -> Result<string option, SuccessInfo option, string, FailureInfo>
}

    
let getInputRepositoryContext connectionString = {
    loadCustomer = fun () -> 
        SQLUtils.getExecuteReaderFunc connectionString
        |> InputRepository.loadCustomers 

    updateImportStatus = fun result -> 
        SQLUtils.getSqlCmdFunc connectionString
        |> InputRepository.updateInputStatus result  //todo: can we get rid of the parentheses by changing param order?
}