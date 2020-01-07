module ImportService

let importCustomers loadCustomersFunc =
    let customers = loadCustomersFunc()
    LoggingService.logRecords customers
    