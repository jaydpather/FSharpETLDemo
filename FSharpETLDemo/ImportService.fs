module ImportService

let importCustomers loadCustomersFunc mappingFunc loggingFunc =
    loadCustomersFunc()
    |> mappingFunc
    |> loggingFunc
    