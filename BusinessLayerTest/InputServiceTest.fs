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
type InputServiceTest_LoadCustomers() =
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

[<TestClass>]
type InputServiceTest_UpdateImportStatus() = 
    [<TestMethod>]
    member this.UpdateSuccessfulRecord_SuccessCase() =
        let repoParam = {
                Action = "xyz"; 
                CustomerNumber = "waz"; 
                CompanyCode = "pbc";
            }
        let serviceParam = NewSuccess repoParam       
        
        let repoCtx = Mock<IInputRepositoryContext>()
                        .Setup(fun x -> <@ x.deleteSuccessfulRecord repoParam @>).Returns(1)
                        .Create()
        let svcCtx = InputServiceFactory.getInputServiceContext repoCtx
        
        let result = svcCtx.updateInputStatus serviceParam
        Mock.Verify(<@ repoCtx.deleteSuccessfulRecord repoParam @>, Times.Once)
        Mock.Verify(<@ repoCtx.updateFailedRecordStatus (any()) @>, Times.Never)

        match result with
        |NewSuccess _ -> 
            let equal = serviceParam = result
            Assert.IsTrue(equal) //should pass param back in success case. todo: why can't I use Assert.AreEqual here? I am returning the same object, so reference equality should work
        |_ -> Assert.Fail("expected original state back");

    [<TestMethod>]
    member this.UpdateSuccessfulRecord_ZeroRowsAffected() =
        let repoParam = {
                Action = "xyz"; 
                CustomerNumber = "waz"; 
                CompanyCode = "pbc";
            }
        let serviceParam = NewSuccess repoParam       
        
        let repoCtx = Mock<IInputRepositoryContext>()
                        .Setup(fun x -> <@ x.deleteSuccessfulRecord repoParam @>).Returns(0)
                        .Create()
        let svcCtx = InputServiceFactory.getInputServiceContext repoCtx
        
        let result = svcCtx.updateInputStatus serviceParam
        Mock.Verify(<@ repoCtx.deleteSuccessfulRecord repoParam @>, Times.Once)
        Mock.Verify(<@ repoCtx.updateFailedRecordStatus (any()) @>, Times.Never)

        match result with
        |NewFailure _ -> Assert.IsTrue(true) //expected result
        |_ -> Assert.Fail("expected NewFailure")

    [<TestMethod>]
    member this.UpdateSuccessfulRecord_DbException() =
        let repoParam = {
                Action = "xyz"; 
                CustomerNumber = "waz"; 
                CompanyCode = "pbc";
            }
        let serviceParam = NewSuccess repoParam       
        
        let repoCtx = Mock<IInputRepositoryContext>()
                        .Setup(fun x -> <@ x.deleteSuccessfulRecord repoParam @>).Raises<Exception>()
                        .Create()
        let svcCtx = InputServiceFactory.getInputServiceContext repoCtx
        
        let result = svcCtx.updateInputStatus serviceParam
        Mock.Verify(<@ repoCtx.deleteSuccessfulRecord repoParam @>, Times.Once)
        Mock.Verify(<@ repoCtx.updateFailedRecordStatus (any()) @>, Times.Never)

        match result with
        |Failure _ -> Assert.IsTrue(true) //expected result
        |_ -> Assert.Fail("expected Failure")

    [<TestMethod>]
    member this.UpdateFailedRecord_SuccessCase() =
        let repoParam = {
                Message = "failed";
                InputStatusUpdateInfo = {
                    CustomerNumber = "abc";
                    CompanyCode = "1223";
                    NextImportStatus = "New";
                }
            }
        let serviceParam = NewFailure repoParam       
        
        let repoCtx = Mock<IInputRepositoryContext>()
                        .Setup(fun x -> <@ x.updateFailedRecordStatus repoParam @>).Returns(1)
                        .Create()
        let svcCtx = InputServiceFactory.getInputServiceContext repoCtx
        
        let result = svcCtx.updateInputStatus serviceParam
        Mock.Verify(<@ repoCtx.deleteSuccessfulRecord (any()) @>, Times.Never)
        Mock.Verify(<@ repoCtx.updateFailedRecordStatus repoParam @>, Times.Once)

        match result with
        |NewFailure _ -> 
            let equal = serviceParam = result
            Assert.IsTrue(equal) //should pass param back in success case. todo: why can't I use Assert.AreEqual here? I am returning the same object, so reference equality should work
        |_ -> Assert.Fail("expected original state back");


    [<TestMethod>]
    member this.UpdateFailedRecord_ZeroRowsAffected() =
        let repoParam = {
                Message = "failed";
                InputStatusUpdateInfo = {
                    CustomerNumber = "abc";
                    CompanyCode = "1223";
                    NextImportStatus = "New";
                }
            }
        let serviceParam = NewFailure repoParam       
        
        let repoCtx = Mock<IInputRepositoryContext>()
                        .Setup(fun x -> <@ x.updateFailedRecordStatus repoParam @>).Returns(0) 
                        .Create()
        let svcCtx = InputServiceFactory.getInputServiceContext repoCtx
        
        let result = svcCtx.updateInputStatus serviceParam
        Mock.Verify(<@ repoCtx.deleteSuccessfulRecord (any()) @>, Times.Never)
        Mock.Verify(<@ repoCtx.updateFailedRecordStatus repoParam @>, Times.Once)

        match result with
        |NewFailure _ -> Assert.IsTrue(true) //expected result
        |_ -> Assert.Fail("expected NewFailure")

    [<TestMethod>]
    member this.UpdateFailedRecord_DbException() =
        let repoParam = {
                Message = "failed";
                InputStatusUpdateInfo = {
                    CustomerNumber = "abc";
                    CompanyCode = "1223";
                    NextImportStatus = "New";
                }
            }
        let serviceParam = NewFailure repoParam       
        
        let repoCtx = Mock<IInputRepositoryContext>()
                        .Setup(fun x -> <@ x.updateFailedRecordStatus repoParam @>).Raises<Exception>()
                        .Create()
        let svcCtx = InputServiceFactory.getInputServiceContext repoCtx
        
        let result = svcCtx.updateInputStatus serviceParam
        Mock.Verify(<@ repoCtx.deleteSuccessfulRecord (any()) @>, Times.Never)
        Mock.Verify(<@ repoCtx.updateFailedRecordStatus repoParam @>, Times.Once)

        match result with
        |Failure _ -> Assert.IsTrue(true) //expected result
        |_ -> Assert.Fail("expected Failure")