﻿module GlobalTypes //todo: rename to State

type Result<'TSuccess,'TFailure1, 'TFailure2> = 
    | Success of 'TSuccess
    | Failure of 'TFailure1
    | NewFailure of 'TFailure2

//todo: break into UniqueIndentifier and NextImportStatus
type InputStatusUpdateInfo = {
    CustomerNumber:string;
    CompanyCode:string;
    NextImportStatus:string; //todo: enum for ImportStatus
}

type FailureInfo = {
    Message:string;
    InputStatusUpdateInfo:InputStatusUpdateInfo;
}
