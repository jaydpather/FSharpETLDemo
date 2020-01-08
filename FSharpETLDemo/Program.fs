open System

[<EntryPoint>]
let main argv =
    
    //todo: create business layer in new project
    //  * this whole method should be the business layer
    //  * main should only call 1 func in business layer
    let sapImportConnectionString = Configuration.ConfigurationManager.ConnectionStrings.["SAPImport"].ConnectionString
    let repoFunc = fun () -> InputRepository.loadCustomers sapImportConnectionString
    let inputServiceFunc = fun () -> InputService.loadCustomers repoFunc

    let mappingFunc = MappingService.mapToWCCustomer DateTime.UtcNow //todo: param will need to be a func that calls DateTime.UtcNow
    
    inputServiceFunc()
    |> mappingFunc
    |> LoggingService.logRecord

    Console.ReadKey()
    0 


(*
REQUIREMENTS:
    * attempt to read 1 record from SAPImport
      * atomic select-update, set Status = 1 (this will be a flag that indicates it's in progress)
    * log success/failure
      * use console logging function for now
*)

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