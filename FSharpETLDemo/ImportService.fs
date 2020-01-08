module ImportService

let importCustomers loadCustomersFunc =
    let customers = loadCustomersFunc() //todo: is it bad style to use let here instead of just returning the function call? using let allows you to see the value in the debugger. or should the dev just go to the call site?
    customers
    