module InputServiceFactory

open Model
open GlobalTypes
open InputRepositoryFactory

//NOTE: creating these ServiceContext types takes way more time b/c you have to declare the type (unlike individual methods)
type InputServiceContext = {
    loadCustomer : unit -> Result<SAPCustomer option, SuccessInfo, string, FailureInfo>;
    updateInputStatus : Result<string option, SuccessInfo, string, FailureInfo> -> Result<string option, SuccessInfo, string, FailureInfo>
}

let getInputServiceContext (inputRepoCtx:InputRepositoryContext) = {
    loadCustomer = fun () -> InputService.loadCustomers inputRepoCtx.loadCustomer;
    updateInputStatus = fun state -> InputService.updateImportStatus inputRepoCtx state
}