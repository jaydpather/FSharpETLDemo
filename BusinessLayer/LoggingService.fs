module LoggingService

open System

open GlobalTypes
open Model

let formatUpdateStatusInfo (updateStatusInfo:InputStatusUpdateInfo) = 
    String.Format("CustomerNumber:{1}{0}CompanyCode:{2}{0}NextImportStatus:{3}", Environment.NewLine, updateStatusInfo.CustomerNumber, updateStatusInfo.CompanyCode, updateStatusInfo.NextImportStatus)

let logNewFailure failureInfo = 
    match failureInfo.InputStatusUpdateInfo with 
    |Some(inputStatusUpdateInfo) -> String.Format("{1}{0}{2}", Environment.NewLine, (formatUpdateStatusInfo inputStatusUpdateInfo), failureInfo.Message) |> Console.WriteLine
    |None _ -> Console.WriteLine (failureInfo.Message)

let logRecord state = 
    match state with 
    | Success (msgOption) ->
        match msgOption with 
        | Some (msg:string) -> Console.WriteLine msg
        | None -> ignore() //this means there were no records to select
    //| Success customerOption -> 
    //    match customerOption with 
    //        | Some (customer:WCCustomer) -> customer |> Console.WriteLine |> ignore 
    //        | None _ -> "no records" |> Console.WriteLine |> ignore
    | Failure (msg:string) -> Console.WriteLine msg
    | NewFailure (fi:FailureInfo) -> logNewFailure fi

let formatExceptionMessage userMsg (ex:Exception) =
    (String.Format("{3}{0}Message:{1}{0}StackTrace:{2}", Environment.NewLine, ex.Message, ex.StackTrace, userMsg))