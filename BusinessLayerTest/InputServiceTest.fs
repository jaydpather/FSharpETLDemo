module InputServiceTest

open System
open Microsoft.VisualStudio.TestTools.UnitTesting

open GlobalTypes
open Model
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
                        .Setup(fun x -> <@ x.loadCustomer () @>).Returns(Some defaultSapCustomer)
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

    [<TestMethod>]
    member this.LoadCustomers_NoCustomerFound() = 
        let repoCtx = Mock<IInputRepositoryContext>()
                        .Setup(fun x -> <@ x.loadCustomer () @>).Returns(None)
                        .Create()
        let svcCtx = InputServiceFactory.getInputServiceContext repoCtx

        let result = svcCtx.loadCustomer ()

        Mock.Verify(<@ repoCtx.loadCustomer @>, Times.Once)
        match result with 
        |Success s -> 
            match s with 
            |Some sapCustomer -> Assert.Fail("expected to receive Success(None) but received Success(Some(Customer))")
            |None _ -> Assert.IsTrue(true) //received expected result
        |_ -> Assert.Fail("expected to receive Success")

    [<TestMethod>]
    member this.LoadCustomers_DbException() = 
        let repoCtx = Mock<IInputRepositoryContext>()
                        .Setup(fun x -> <@ x.loadCustomer () @>).Raises<Exception>()
                        .Create()
        let svcCtx = InputServiceFactory.getInputServiceContext repoCtx

        let result = svcCtx.loadCustomer ()

        Mock.Verify(<@ repoCtx.loadCustomer @>, Times.Once)
        match result with 
        |Success s -> 
            Assert.Fail("expected to receive Failure but received Success")
        |Failure _ -> Assert.IsTrue(true) //expected result
