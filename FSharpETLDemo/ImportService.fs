module ImportService

let importCustomers loadCustomersFunc =
    let customers = loadCustomersFunc //todo: how does the compiler know this is a function call and not a delegate assignment?
    customers
    