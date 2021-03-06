﻿module Model

open System

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
    CustomerNumber:string;
    CompanyCode:string;
    Name:string;
    Address_City:string;
    Address_CountryCode:string;
    Phone:string;
    VATCode:string;
    LanguageCode:string;
    Timestamp:DateTime; //this is nullable in the db, but we'll always have a value since we're inserting
    IsActive:bool;
    Address_StreetHouseNumber:string;
    Address_PostalCode:string;
    Address_Region:string;
    Address_CustomerType:string;
}

type LogRecord = {
    SAPCustomer:SAPCustomer;
    Message:string
}