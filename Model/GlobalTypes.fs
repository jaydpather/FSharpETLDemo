﻿module GlobalTypes

type Result<'TSuccess1, 'TSuccess2, 'TFailure1, 'TFailure2> = //todo*: rename Result to State
    | Success of 'TSuccess1
    | NewSuccess of 'TSuccess2
    | Failure of 'TFailure1
    | NewFailure of 'TFailure2

//todo: break into UniqueIndentifier and NextImportStatus
type InputStatusUpdateInfo = {
    CustomerNumber:string;
    CompanyCode:string;
    NextImportStatus:string; //todo*: enum for ImportStatus
}

type FailureInfo = {
    Message:string;
    InputStatusUpdateInfo:InputStatusUpdateInfo;
}

type SuccessInfo = {
    Action: string;
    CustomerNumber: string;
    CompanyCode: string;
}