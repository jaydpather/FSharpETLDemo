module InputRepositoryFactory

open GlobalTypes
open Model

type InputRepositoryContext = { 
    loadCustomer : unit -> SAPCustomer option;
    updateImportStatus : Result<string option, string, FailureInfo> -> Result<string option, string, FailureInfo>
}

    
let getInputRepositoryContext connectionString = {
    loadCustomer = fun () -> InputRepository.loadCustomers connectionString;
    updateImportStatus = fun result -> InputRepository.updateInputStatus connectionString result
}