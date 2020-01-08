module ImportService

let importCustomers loadCustomersFunc =
    let customers = loadCustomersFunc()
    customers
    