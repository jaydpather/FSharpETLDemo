module internal InputRepository //todo: rename to SAPImportRepository?

open System
open System.Data.SqlClient

open GlobalTypes
open Model

//todo: private methods in all modules

let populateSAPCustomer (dataReader:SqlDataReader) =
    dataReader.Read() |> ignore 
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



let loadCustomers connectionString = 
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
    //WHERE CC.CompanyCode IN ('W031','TH31','INLC','TH90','TH47','CK07','CK47','PB31','3906','PVHE')
    
    //todo: reusable method for command and connection
    use sqlConn = new SqlConnection(connectionString)
    sqlConn.Open()
    use sqlCmd = new SqlCommand(query, sqlConn) //todo: does F# have String.Empty?
    use dataReader = sqlCmd.ExecuteReader();

    //NOTE: it seems that if a string is null in the DB, it comes out as "" in F#, so there is no need to check for null
    //  * this also means if you want your F# code to distinguish between NULL and "", you need to use a SQL CASE statement
    let hasRowsFunc = fun () -> dataReader.HasRows //using a delegate so it's injectable
    let populateObjFunc = fun () -> populateSAPCustomer dataReader //using a delegate so it's injectable
    let loadedCustomer = readFromDataReader hasRowsFunc populateObjFunc
        
    loadedCustomer


//////////////////////////////////////////////////////////////////////////

let private updateFailedRecord connectionString failureInfo = 
    let query = "
declare @importStatusId int = (select Id from ImportStatus where StatusName = @importStatusName)
update CustomerCompany 
    set ImportStatusId = @importStatusId 
where CustomerNumber = @customerNumber
and CompanyCode = @companyCode"
    use sqlConn = new SqlConnection(connectionString)
    sqlConn.Open()
    use sqlCmd = new SqlCommand(query, sqlConn) //todo: does F# have String.Empty?
    sqlCmd.Parameters.Add(new SqlParameter("importStatusName", failureInfo.InputStatusUpdateInfo.NextImportStatus)) |> ignore
    sqlCmd.Parameters.Add(new SqlParameter("customerNumber", failureInfo.InputStatusUpdateInfo.CustomerNumber)) |> ignore
    sqlCmd.Parameters.Add(new SqlParameter("companyCode", failureInfo.InputStatusUpdateInfo.CompanyCode)) |> ignore

    let rowsUpdated = sqlCmd.ExecuteNonQuery();
    match rowsUpdated with 
    |1 -> NewFailure failureInfo //pass the original failure back to logging service
    |_ -> NewFailure ({failureInfo with
                        Message = String.Format("Failed to update ImportStatus of input record.{0}{1}", Environment.NewLine, failureInfo.Message)
                        }:FailureInfo)



let updateInputStatus connectionString state = 
    match state with 
    |Success(s) -> state
    |Failure s -> state
    |NewFailure failureInfo -> updateFailedRecord connectionString failureInfo