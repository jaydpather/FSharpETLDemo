module GlobalTypes //todo: rename to State

type Result<'TSuccess,'TFailure1, 'TFailure2> = 
    | Success of 'TSuccess
    | Failure of 'TFailure1
    | NewFailure of 'TFailure2

type InputStatusUpdateInfo = {
    CustomerNumber:string;
    CompanyCode:string;
    NextImportStatus:string;
}

type FailureInfo = {
    Message:string;
    InputStatusUpdateInfo:InputStatusUpdateInfo option;
}
