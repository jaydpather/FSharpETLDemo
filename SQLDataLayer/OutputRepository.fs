module OutputRepository

open System.Data.SqlClient

open GlobalTypes
open System
open Model

let upsertCustomer connectionString (customer:WCCustomer) =
    //todo: thread-safe CustomerId in WeConnectSales DB
    let query = @"
DECLARE @Actions TABLE(Action VARCHAR(20));  --not sure why we need 20 length, but that's what the MS examples have
MERGE Customers t 
    USING (select @customerNumber as CustomerNumber, 
                    @countryCode as CountryCode, 
                    @name as Name, 
                    @city as City, 
                    @postalCode as PostalCode, 
                    @region as Region, 
                    @languageCode as LanguageCode, 
                    @vatCode as VatCode, 
                    @streetHouseNumber as StreetHouseNumber, 
                    @phone as Phone, 
                    @timestamp as Timestamp, 
                    @isActive as IsActive, 
                    @companyCode as CompanyCode, 
                    @customerType as CustomerType) s
ON (t.CustomerNumber = s.CustomerNumber and t.CompanyCode = s.CompanyCode)
WHEN MATCHED
    THEN UPDATE SET 
        t.[CustomerNumber] = s.CustomerNumber,
	    t.[CompanyCode] = s.CompanyCode,
	    t.[Name] = s.Name,
	    t.[Address_City] = s.City,
	    t.[Address_CountryCode] = s.CountryCode,
	    t.[Phone] = s.Phone,
	    t.[VATCode] = s.VatCode,
	    t.[LanguageCode] = s.LanguageCode,
	    t.[Timestamp] = s.Timestamp,
	    t.[IsActive] = s.IsActive,
	    t.[Address_StreetHouseNumber] = s.StreetHouseNumber,
	    t.[Address_PostalCode] = s.PostalCode,
	    t.[Address_Region] = s.Region,
	    t.[CustomerType] = s.CustomerType
WHEN NOT MATCHED BY TARGET 
    THEN INSERT ([Id]
				   ,[CustomerNumber]
				   ,[CompanyCode]
				   ,[Name]
				   ,[Address_City]
				   ,[Address_CountryCode]
				   ,[Phone]
				   ,[VATCode]
				   ,[LanguageCode]
				   ,[Timestamp]
				   ,[IsActive]
				   ,[Address_StreetHouseNumber]
				   ,[Address_PostalCode]
				   ,[Address_Region]
				   ,[CustomerType])
         VALUES ((SELECT ISNULL((MAX(Id) + 1),1000) FROM [Customers]) --todo: thread-safe Id
					,s.CustomerNumber
					,s.CompanyCode
					,s.Name
					,s.City 
					,s.CountryCode
					,s.Phone
					,s.VatCode
					,s.LanguageCode
					,s.Timestamp
					,s.IsActive
					,s.StreetHouseNumber
					,s.PostalCode
					,s.Region
					,s.CustomerType)
output $action into @Actions;

select Action from @Actions --impossible for this to return more than 1 row, since we selected from parameters"
    use sqlConn = new SqlConnection(connectionString)
    sqlConn.Open()
    use sqlCmd = new SqlCommand(query, sqlConn)

    sqlCmd.Parameters.Add(new SqlParameter("customerNumber", customer.CustomerNumber)) |> ignore
    sqlCmd.Parameters.Add(new SqlParameter("city", customer.Address_City)) |> ignore
    sqlCmd.Parameters.Add(new SqlParameter("countryCode", customer.Address_CountryCode)) |> ignore
    sqlCmd.Parameters.Add(new SqlParameter("customerType", customer.Address_CustomerType)) |> ignore
    sqlCmd.Parameters.Add(new SqlParameter("postalCode", customer.Address_PostalCode)) |> ignore
    sqlCmd.Parameters.Add(new SqlParameter("region", customer.Address_Region)) |> ignore
    sqlCmd.Parameters.Add(new SqlParameter("streetHouseNumber", customer.Address_StreetHouseNumber)) |> ignore
    sqlCmd.Parameters.Add(new SqlParameter("companyCode", customer.CompanyCode)) |> ignore
    sqlCmd.Parameters.Add(new SqlParameter("isActive", customer.IsActive)) |> ignore
    sqlCmd.Parameters.Add(new SqlParameter("languageCode", customer.LanguageCode)) |> ignore
    sqlCmd.Parameters.Add(new SqlParameter("name", customer.Name)) |> ignore
    sqlCmd.Parameters.Add(new SqlParameter("phone", customer.Phone)) |> ignore
    sqlCmd.Parameters.Add(new SqlParameter("timestamp", customer.Timestamp)) |> ignore
    sqlCmd.Parameters.Add(new SqlParameter("vatCode", customer.VATCode)) |> ignore

    let sqlAction = sqlCmd.ExecuteScalar() :?> string;
    let result = String.Format("{0} CustomerNumber:{1} CompanyCode:{2}", sqlAction, customer.CustomerNumber, customer.CompanyCode)
    
    result
    