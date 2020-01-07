module Model

type SAPCustomer = {
    CustomerNumber:string;
    CompanyCode:string;
}

type WCCustomer = {
    CustomerId:int;
    CustomerNumber:string;
    CompanyCode:string;
}

type LogRecord = {
    SAPCustomer:SAPCustomer;
    Message:string
}