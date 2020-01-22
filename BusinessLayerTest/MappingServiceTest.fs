namespace BusinessLayerTest

open System
open Microsoft.VisualStudio.TestTools.UnitTesting

open GlobalTypes
open Model

[<TestClass>]
type TestClass () =


    let expectedFieldsMatch (sapCustomer:SAPCustomer) wcCustomer = 
        Assert.AreEqual(sapCustomer.CustomerNumber, wcCustomer.CustomerNumber);
        Assert.AreEqual(sapCustomer.CompanyCode, wcCustomer.CompanyCode);
        Assert.AreEqual(sapCustomer.City, wcCustomer.Address_City);
        Assert.AreEqual(sapCustomer.CountryCode, wcCustomer.Address_CountryCode);
        Assert.AreEqual(sapCustomer.CustomerType, wcCustomer.Address_CustomerType);
        Assert.AreEqual(sapCustomer.LanguageCode, wcCustomer.LanguageCode);
        Assert.AreEqual(sapCustomer.Name, wcCustomer.Name);
        Assert.AreEqual(sapCustomer.Phone, wcCustomer.Phone);
        Assert.AreEqual(sapCustomer.PostalCode, wcCustomer.Address_PostalCode);
        Assert.AreEqual(sapCustomer.Region, wcCustomer.Address_Region);
        Assert.AreEqual(sapCustomer.StreetHouseNumber, wcCustomer.Address_StreetHouseNumber);
        Assert.AreEqual(sapCustomer.VATNumber, wcCustomer.VATCode);

    [<TestMethod>]
    member this.MapToWCCustomer_ExpectedFieldsMatch() =
        let sapCustomer = {
            CustomerNumber = "CN1234";
            CountryCode = "CountryCode";
            Name = "Name";
            City = "City";
            PostalCode = "Postal Code";
            Region = "Region";
            LanguageCode = "LC";
            VATNumber = "VATNumber";
            StreetHouseNumber = "123";
            Phone = "Ph";
            IsDeleted = "";
            CompanyCode = "CC";
            CustomerType = "ct"
        }
        let result = MappingService.mapToWCCustomer DateTime.Now sapCustomer
        
        match result with 
        |Success(s) -> 
            match s with
            | Some(wcCustomer) -> expectedFieldsMatch sapCustomer wcCustomer
            | None _ -> Assert.Fail("expected Success(Some), but got Success(None)");
        |Failure _ -> Assert.Fail("expected Success, but got Failure")
        
        //todo: check expected value of IsDeleted

