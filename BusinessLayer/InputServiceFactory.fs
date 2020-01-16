module InputServiceFactory

open Model
open GlobalTypes

type InputServiceContext = {
    loadCustomer : unit -> Result<SAPCustomer option, string, FailureInfo>
}

let getInputServiceContext loadCustomerFunc = {
    loadCustomer = fun () -> InputService.loadCustomers loadCustomerFunc
}