open System

[<EntryPoint>]
let main argv =
    //todo: use VS Code instead of VS
    //  * then you can switch to Mac

    //todo: 

    //todo: learn how to compile to exe with dotnet publish -c release -r win-x64
    //  * so far this command freezes

    //todo: create business layer in new project
    //  * this whole method should be the business layer
    //  * main should only call 1 func in business layer

    //todo: create Factories modules inside DB layer and business layer (1 for services, 1 for repos)
    //todo: create types called InputService/Repository, OutputService/Repository, and LoggingService/Repository
    //  * each type contains delegates for functions like save, load, etc.
    //  * e.g., InputRepository would have functions like LoadCustomers and DeleteSuccessfulCustomers

    let sapImportConnectionString = Configuration.ConfigurationManager.ConnectionStrings.["SAPImport"].ConnectionString
    let inputRepoLoadFunc = fun () -> InputRepository.loadCustomers sapImportConnectionString
    let inputServiceLoadFunc = fun () -> InputService.loadCustomers inputRepoLoadFunc
    let inputRepoUpdateStatusDBFunc = InputRepository.updateFailedRecord sapImportConnectionString
    let inputRepoUpdateStatusFunc = fun state -> InputRepository.updateInputStatus inputRepoUpdateStatusDBFunc state
    let inputServiceUpdateStatusFunc = fun state -> InputService.updateImportStatus inputRepoUpdateStatusFunc state


    let wcSalesConnectionString = Configuration.ConfigurationManager.ConnectionStrings.["WeConnectSales"].ConnectionString
    let outputRepoFunc = fun customer -> OutputRepository.upsertCustomer wcSalesConnectionString customer
    let outputServiceSaveFunc = fun customer -> OutputService.saveCustomer outputRepoFunc customer 

    //todo: if we need a more accurate timestamp, we could return to this file (right before mapping) in order to access DateTime.UtcNow
    //  * or, we could create a delegate for a callback that returns DateTime.UtcNow. but is that bad style? (make a forum post)
    let mappingFunc = MappingService.mapToWCCustomer DateTime.UtcNow 
    
    while(true) do
        ImportWorkflow.importCustomers inputServiceLoadFunc mappingFunc outputServiceSaveFunc inputServiceUpdateStatusFunc LoggingService.logRecord

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