module Model

type SAPCustomer = {
    CustomerNumber:string;
    CountryCode:string;
    Name:string;
    City:string;
    PostalCode:string;
    Region:string;
    LanguageCode:string;
    VATNumber:string;
    StreetHouseNumber:string;
    Phone:string;
    IsDeleted:string;
    CompanyCode:string;
    CustomerType:string;
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