module ImportService

let importCustomers () =
    let customers = InputService.loadCustomers()
    LoggingService.logRecords customers
    