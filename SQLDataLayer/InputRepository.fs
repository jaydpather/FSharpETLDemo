module internal InputRepository

open System
open System.Data.SqlClient

open GlobalTypes
open Model

//todo: private methods in all modules

let populateSAPCustomer (dataReader:SqlDataReader) =
    dataReader.Read() |> ignore 
    //NOTE: it seems that if a string is null in the DB, it comes out as "" in F#, so there is no need to check for null
        //  * this also means if you want your F# code to distinguish between NULL and "", you need to use a SQL CASE statement
    {
        CustomerNumber = (string)dataReader.["CustomerNumber"]; 
        CompanyCode = (string)dataReader.["CompanyCode"];
        Name = (string)dataReader.["Name"];
        City = (string)dataReader.["City"];
        PostalCode = (string)dataReader.["PostalCode"];
        Region = (string)dataReader.["Region"];
        LanguageCode = (string)dataReader.["LanguageCode"];
        VATNumber =(string)dataReader.["VATNumber"];
        StreetHouseNumber = (string)dataReader.["StreetHouseNumber"];
        Phone = (string)dataReader.["Phone"];
        IsDeleted = (string)dataReader.["IsDeleted"];
        CustomerType = (string)dataReader.["CustomerType"];
        CountryCode = (string)dataReader.["CountryCode"];
    }

let readFromDataReader dataReaderHasRowsFunc populateObjFunc = 
    match dataReaderHasRowsFunc() with 
        | true -> Some(populateObjFunc())
        | false -> None



let loadCustomers executeReaderFunc = 
    //todo: filtering, CASE, and TRIM should all be done in business layer
    let query = @"
declare @FirstRecord table --we have to use a table var b/c the primary key is 2 columns
(
	CustomerNumber nchar(10),
	CompanyCode nchar(6)
)

declare @statusIdNew int = (select Id from ImportStatus ist where ist.StatusName = 'New')
declare @statusIdInProgress int = (select Id from ImportStatus ist where ist.StatusName = 'In Progress')

insert into @FirstRecord(CustomerNumber, CompanyCode)
	(select top(1) cc.CustomerNumber, cc.CompanyCode
	from CustomerCompany cc
	where cc.ImportStatusId = @statusIdNew)
	--todo: why can't I do ORDER BY here?

declare @updateCount int = 0

update CustomerCompany 
set ImportStatusId = @statusIdInProgress
from CustomerCompany cc
join @FirstRecord fr on fr.CustomerNumber = cc.CustomerNumber
	and fr.CompanyCode = cc.CompanyCode
where cc.ImportStatusId = @statusIdNew --filtering on New ImportStatus so that only 1 thread can update

set @updateCount = @@ROWCOUNT --if multiple threads try to update, only 1 will get @updateCount = 1. the rest will get 0

SELECT top(1)
      LTRIM(RTRIM(CAST(CB.[CustomerNumber] AS NVARCHAR(10)))) AS [CustomerNumber]
      ,CB.[CountryCode]
      ,CB.[Name]
      ,CB.[City]
      ,CB.[PostalCode]
      ,CB.[Region]
      ,CB.[LanguageCode]
      ,CB.[VATNumber]
      ,CB.[StreetHouseNumber]
      ,CB.[Phone]
      ,CB.[Timestamp]
	  ,CASE WHEN LEN(CB.[CustomerType])=0 OR CB.[CustomerType] IS NULL THEN '00' ELSE CB.[CustomerType] END as CustomerType
      ,CASE LEN(CB.[IsDeleted]) WHEN 0 THEN NULL ELSE CB.[IsDeleted] END as IsDeleted
	  ,CC.CompanyCode
	  ,C.[Id] As CustomerId  
	FROM [dbo].[CustomerBasic] CB
	LEFT JOIN [dbo].[CustomerCompany] CC ON CC.CustomerNumber=CB.CustomerNumber
	LEFT OUTER JOIN [WeConnectSales_Monkey].[dbo].[Customers] C ON (C.CustomerNumber=CB.CustomerNumber AND C.CompanyCode=CC.CompanyCode)
	join @FirstRecord fr on fr.CustomerNumber = CC.CustomerNumber and fr.CompanyCode = CC.CompanyCode
	WHERE @updateCount = 1 --this ensures only 1 thread can handle a record at a time";

    let dataReaderCallback = fun (dataReader:SqlDataReader) -> 
        let hasRowsFunc = fun () -> dataReader.HasRows //using a delegate so it's injectable
        let populateObjFunc = fun () -> populateSAPCustomer dataReader //using a delegate so it's injectable
        readFromDataReader hasRowsFunc populateObjFunc
    //WHERE CC.CompanyCode IN ('W031','TH31','INLC','TH90','TH47','CK07','CK47','PB31','3906','PVHE')
    
    executeReaderFunc query dataReaderCallback
    
//////////////////////////////////////////////////////////////////////////

let updateFailedRecord failureInfo getSqlCmdFunc = 
    let query = "
declare @importStatusId int = (select Id from ImportStatus where StatusName = @importStatusName)
update CustomerCompany 
    set ImportStatusId = @importStatusId 
where CustomerNumber = @customerNumber
and CompanyCode = @companyCode"
    
    let dbCallback (sqlCmd:SqlCommand) = 
        sqlCmd.Parameters.Add(new SqlParameter("importStatusName", failureInfo.InputStatusUpdateInfo.NextImportStatus)) |> ignore
        sqlCmd.Parameters.Add(new SqlParameter("customerNumber", failureInfo.InputStatusUpdateInfo.CustomerNumber)) |> ignore
        sqlCmd.Parameters.Add(new SqlParameter("companyCode", failureInfo.InputStatusUpdateInfo.CompanyCode)) |> ignore

        //todo: this method shoud return RowCount. Service should convert to Failure
        let rowsUpdated = sqlCmd.ExecuteNonQuery();
        match rowsUpdated with 
        |1 -> NewFailure failureInfo //pass the original failure back to logging service
        |_ -> NewFailure ({failureInfo with
                            Message = String.Format("Failed to update ImportStatus of input record.{0}{1}", Environment.NewLine, failureInfo.Message)
                            }:FailureInfo)
        //note: patterns are tested in order, so we must check for 1 before _

    getSqlCmdFunc query dbCallback

//////////////////////////////////////////////////////////////////////////////
let deleteSuccessfulRecord (successInfo:SuccessInfo) getSqlCmdFunc = 
    let query = "delete from CustomerCompany where CustomerNumber = @CustomerNumber and CompanyCode = @CompanyCode"
    //todo: also delete CustomerBasic, if no CustomerCompany records left AND it's not due to SAP sending only CustomerBasic
    let dbCallback (sqlCmd:SqlCommand) =
        sqlCmd.Parameters.Add(new SqlParameter("CustomerNumber", successInfo.CustomerNumber)) |> ignore
        sqlCmd.Parameters.Add(new SqlParameter("CompanyCode", successInfo.CompanyCode)) |> ignore
        let rowsAffected = sqlCmd.ExecuteNonQuery()

        //todo: this method should return row count. Service should convert to Failure
        match rowsAffected with
        |1 -> NewSuccess successInfo
        |rowCount -> NewFailure { 
                Message = String.Format("Customer saved in WC successfully, but failed to delete input record. Expected 1 row affected, but {0} rows were affected.", rowCount);
                InputStatusUpdateInfo = {
                    CustomerNumber = successInfo.CustomerNumber;
                    CompanyCode = successInfo.CompanyCode;
                    NextImportStatus = "Failed"; //todo: remove this dummy value
                }
            }

    getSqlCmdFunc query dbCallback


