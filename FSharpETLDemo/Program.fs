// Learn more about F# at http://fsharp.org

open System

open ImportService

[<EntryPoint>]
let main argv =
    //printfn "Hello World from F#!"
    ImportService.importCustomers()
    Console.ReadKey()
    0 // return an integer exit code


(*
REQUIREMENTS:
    * attempt to read 1 record from SAPImport
      * atomic select-update, set Status = 1 (this will be a flag that indicates it's in progress)
    * log success/failure
      * use console logging function for now
*)