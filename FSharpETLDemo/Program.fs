﻿open System

let applyIt op arg = op arg

[<EntryPoint>]
let main argv =
    
    InputRepository.loadCustomers 
    |> InputService.loadCustomers 
    |> ImportService.importCustomers
    |> LoggingService.logRecords 

    Console.ReadKey()
    0 


(*
REQUIREMENTS:
    * attempt to read 1 record from SAPImport
      * atomic select-update, set Status = 1 (this will be a flag that indicates it's in progress)
    * log success/failure
      * use console logging function for now
*)