module OutputRepositoryFactory

open Model

type OutputRepositoryContext = {
    saveCustomer: WCCustomer -> string
}

let getOutputRepositoryContext connectionString = {
    saveCustomer = fun customer -> OutputRepository.upsertCustomer connectionString customer
}