module InputRepository

open System
open System.Data.SqlClient

open GlobalTypes
open Model

let loadCustomers () = 
    //todo: filtering, CASE, and TRIM should all be done in business layer
    //  * change inner join to LEFT join - business layer will filter out records with a missing CustomerCompany
    let query = @"SELECT top(1)
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
	INNER JOIN [dbo].[CustomerCompany] CC ON CC.CustomerNumber=CB.CustomerNumber
	LEFT OUTER JOIN [WeConnectSales_Monkey].[dbo].[Customers] C ON (C.CustomerNumber=CB.CustomerNumber AND C.CompanyCode=CC.CompanyCode)
	WHERE CC.CompanyCode IN ('W031','TH31','INLC','TH90','TH47','CK07','CK47','PB31','3906','PVHE')"
    
    //todo: reusable method for command and connection
    use sqlConn = new SqlConnection("Server=AMSAPP012\Dev;Initial Catalog=SAPImport_Monkey;Trusted_Connection=true;") //todo: connection string in app.config
    sqlConn.Open()
    use sqlCmd = new SqlCommand(query, sqlConn) //todo: does F# have String.Empty?
    use dataReader = sqlCmd.ExecuteReader();
    
    //todo: actually don't need this sequence thing since we're only selecting 1 record
    //todo: return 1 SAPCustomer OPTION instead of a SAPCustomer list. (you need option in case it loaded 0 records)
    let records = seq {//todo: what is the difference between Seq and List?
        while(dataReader.Read()) do //todo: seems like you have to use a while loop here, b/c that's how DataReader works. is there an F# data reader?
            yield {
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
    }
    Success (Seq.toList records) //todo: does Seq.toList create a new list?