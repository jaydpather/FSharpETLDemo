module ImportService

open StateChecker

let importCustomers loadCustomersFunc mappingFunc loggingFunc =
    loadCustomersFunc()
    |> checkState mappingFunc
    |> loggingFunc
    