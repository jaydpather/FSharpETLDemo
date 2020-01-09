open System

[<EntryPoint>]
let main argv =
    
    //todo: create business layer in new project
    //  * this whole method should be the business layer
    //  * main should only call 1 func in business layer
    let sapImportConnectionString = Configuration.ConfigurationManager.ConnectionStrings.["SAPImport"].ConnectionString
    let repoFunc = fun () -> InputRepository.loadCustomers sapImportConnectionString
    let inputServiceFunc = fun () -> InputService.loadCustomers repoFunc

    //todo: if we need a more accurate timestamp, we could return to this file (right before mapping) in order to access DateTime.UtcNow
    //  * or, we could create a delegate for a callback that returns DateTime.UtcNow. but is that bad style? (make a forum post)
    let mappingFunc = MappingService.mapToWCCustomer DateTime.UtcNow 
    
    ImportService.importCustomers inputServiceFunc mappingFunc LoggingService.logRecord

    Console.ReadKey()
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