module OutputRepositoryFactory

open Model

type OutputRepositoryContext = {
    saveCustomer: WCCustomer -> string
}

let getOutputRepositoryContext connectionString = {
    saveCustomer = fun customer -> 
        SQLUtils.getSqlCmdFunc connectionString
        |> OutputRepository.upsertCustomer customer
}