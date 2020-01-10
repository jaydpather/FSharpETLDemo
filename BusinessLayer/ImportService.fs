module ImportService

open StateChecker

let importCustomers loadCustomersFunc mappingFunc saveCustomersFunc loggingFunc =
    loadCustomersFunc()
    |> checkState mappingFunc
    |> checkState saveCustomersFunc
    |> loggingFunc
    