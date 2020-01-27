open System

[<EntryPoint>]
let main argv =
    //todo: use VS Code instead of VS
    //  * then you can switch to Mac

    //todo: CustomerCompany.ImportStatusId needs to be in a new table (CompanyCustomerImportStatus), so SAP team will not be affected by it
    //todo: make a stored view that joins CustomerBasic, CustomerCompany, and CustomerCompanyImportStatus
    //todo: use IDENTITY INSERT in your SQL deployment scripts, so you can just pass StatusIds to queries. (StatusIds stored in an enum)

    let inputServiceCtx = 
        Configuration.ConfigurationManager.ConnectionStrings.["SAPImport"].ConnectionString 
        |> InputRepositoryFactory.getInputRepositoryContext 
        |> InputServiceFactory.getInputServiceContext

    let outputSvcCtx = 
        Configuration.ConfigurationManager.ConnectionStrings.["WeConnectSales"].ConnectionString
        |> OutputRepositoryFactory.getOutputRepositoryContext 
        |> OutputServiceFactory.getOutputServiceContext 

    //todo: if we need a more accurate timestamp, we could return to this file (right before mapping) in order to access DateTime.UtcNow
    //  * or, we could create a delegate for a callback that returns DateTime.UtcNow. but is that bad style? (make a forum post)
    let mappingFunc = MappingService.mapToWCCustomer DateTime.UtcNow 
    
    while(true) do
        ImportWorkflow.importCustomers inputServiceCtx mappingFunc outputSvcCtx LoggingService.logRecord

    //Console.ReadKey()
    0 


(*
OUTPUT DATA LAYER
param: state
    if state = failure
        state
    else
        try
            write to WCSales
            Success ()
        catch
            Failure "ex message, formatted"

Logging layer will never receive Success(SAPCustomer), b/c we don't want to log anything for a success
  * adjust logging layer as necessary
*)