module InputServiceTest
//namespace BusinessLayerTest

open System
open Microsoft.VisualStudio.TestTools.UnitTesting

open GlobalTypes
open Model
open Foq.Linq
open InputRepositoryFactory
open Foq


let defaultSapCustomer = {
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

[<TestClass>]
type InputServiceTest () =
    [<TestMethod>]
    member this.LoadCustomers_SuccessCase() = 
        let repoCtx = Mock<IInputRepositoryContext>()
                        .Setup(fun x -> <@ x.loadCustomer() @>).Returns(Some defaultSapCustomer)
                        .Create()
        let svcCtx = InputServiceFactory.getInputServiceContext repoCtx

        let result = svcCtx.loadCustomer ()

        Mock.Verify(<@ repoCtx.loadCustomer @>, Times.Once)
        match result with 
        |Success s -> 
            match s with 
            |Some sapCustomer -> Assert.IsTrue(true) //received expected result
            |None _ -> Assert.Fail("expected to receive Success(Some(Customer)) but received Success(None)")
        |_ -> Assert.Fail("expected to receive Success")