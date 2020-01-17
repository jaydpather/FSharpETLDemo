module LoggingService

open System

open GlobalTypes
open Model

let formatUpdateStatusInfo (updateStatusInfo:InputStatusUpdateInfo) = 
    String.Format("CustomerNumber:{1}{0}CompanyCode:{2}{0}NextImportStatus:{3}", Environment.NewLine, updateStatusInfo.CustomerNumber, updateStatusInfo.CompanyCode, updateStatusInfo.NextImportStatus)

let logNewFailure failureInfo = 
    String.Format("{1}{0}{2}", Environment.NewLine, (formatUpdateStatusInfo failureInfo.InputStatusUpdateInfo), failureInfo.Message) |> Console.WriteLine

let logRecord state = 
    match state with 
    | Success (msgOption) ->
        match msgOption with 
        | Some (s:string) -> Console.WriteLine s
        | None -> ignore() //this means there were no records to select
    | NewSuccess (siOption) ->
        match siOption with 
        | Some (si:SuccessInfo) -> 
            String.Format("SUCCESS: {0} {1} {2}", si.Action, si.CustomerNumber, si.CompanyCode) 
            |> Console.WriteLine 
        | None -> ignore() //this means there were no records to select
    //| Success customerOption -> 
    //    match customerOption with 
    //        | Some (customer:WCCustomer) -> customer |> Console.WriteLine |> ignore 
    //        | None _ -> "no records" |> Console.WriteLine |> ignore
    | Failure (msg:string) -> Console.WriteLine msg
    | NewFailure (fi:FailureInfo) -> logNewFailure fi

let formatExceptionMessage userMsg (ex:Exception) =
    (String.Format("{3}{0}Message:{1}{0}StackTrace:{2}", Environment.NewLine, ex.Message, ex.StackTrace, userMsg))