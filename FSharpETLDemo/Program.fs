open System

[<EntryPoint>]
let main argv =
    
    //todo: create business layer in new project
    //  * this whole method should be the business layer
    //  * main should only call 1 func in business layer
    let repoFunc = fun () -> InputRepository.loadCustomers() 
    let inputServiceFunc = fun () -> InputService.loadCustomers repoFunc
    
    inputServiceFunc
    |> ImportService.importCustomers
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