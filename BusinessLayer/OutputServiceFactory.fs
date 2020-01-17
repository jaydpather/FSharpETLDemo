module OutputServiceFactory

open Model
open GlobalTypes

type OutputServiceContext = {
    saveCustomer : WCCustomer -> Result<string option, SuccessInfo option, string, FailureInfo>
}

let getOutputServiceContext outputRepoCtx = {
    saveCustomer = OutputService.saveCustomer outputRepoCtx
}


