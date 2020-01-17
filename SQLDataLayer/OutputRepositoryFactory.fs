module OutputRepositoryFactory

open Model
open GlobalTypes

type OutputRepositoryContext = {
    saveCustomer: WCCustomer -> SuccessInfo
}

let getOutputRepositoryContext connectionString = {
    saveCustomer = fun customer -> 
        SQLUtils.getSqlCmdFunc connectionString
        |> OutputRepository.upsertCustomer customer
}