module MappingService

open Model
open GlobalTypes

let convertToIsActive isDeleted = //todo: implement method
    true

let mapToWCCustomer timestampOfBatch (sapCustomer:SAPCustomer) =
    Success (Some {
            CustomerNumber = sapCustomer.CustomerNumber;
            CompanyCode = sapCustomer.CompanyCode;
            Name = sapCustomer.Name;
            Address_City = sapCustomer.City;
            Address_CountryCode = sapCustomer.CountryCode;
            Phone = sapCustomer.Phone;
            VATCode = sapCustomer.VATNumber;
            LanguageCode = sapCustomer.LanguageCode;
            Timestamp = timestampOfBatch;
            IsActive = convertToIsActive sapCustomer.IsDeleted;
            Address_StreetHouseNumber = sapCustomer.StreetHouseNumber;
            Address_PostalCode = sapCustomer.PostalCode;
            Address_Region = sapCustomer.Region;
            Address_CustomerType = sapCustomer.CustomerType;
        })