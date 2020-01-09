module OutputRepository

open System.Data.SqlClient

open GlobalTypes
open System
open Model

let upsertCustomer connectionString (customer:WCCustomer) =
    let query = @""
    use sqlConn = new SqlConnection(connectionString)
    sqlConn.Open()
    use sqlCmd = new SqlCommand(query, sqlConn)

    sqlCmd.Parameters.Add(new SqlParameter("customerNumber", customer.CustomerNumber)) |> ignore
    sqlCmd.Parameters.Add(new SqlParameter("addressCity", customer.Address_City)) |> ignore
    sqlCmd.Parameters.Add(new SqlParameter("addressCountryCode", customer.Address_CountryCode)) |> ignore
    sqlCmd.Parameters.Add(new SqlParameter("addressCustomerType", customer.Address_CustomerType)) |> ignore
    sqlCmd.Parameters.Add(new SqlParameter("addressPostalCode", customer.Address_PostalCode)) |> ignore
    sqlCmd.Parameters.Add(new SqlParameter("addressRegion", customer.Address_Region)) |> ignore
    sqlCmd.Parameters.Add(new SqlParameter("addressStreetHouseNumber", customer.Address_StreetHouseNumber)) |> ignore
    sqlCmd.Parameters.Add(new SqlParameter("companyCode", customer.CompanyCode)) |> ignore
    sqlCmd.Parameters.Add(new SqlParameter("isActive", customer.IsActive)) |> ignore
    sqlCmd.Parameters.Add(new SqlParameter("languageCode", customer.LanguageCode)) |> ignore
    sqlCmd.Parameters.Add(new SqlParameter("name", customer.Name)) |> ignore
    sqlCmd.Parameters.Add(new SqlParameter("phone", customer.Phone)) |> ignore
    sqlCmd.Parameters.Add(new SqlParameter("timestamp", customer.Timestamp)) |> ignore
    sqlCmd.Parameters.Add(new SqlParameter("vatCode", customer.VATCode)) |> ignore

    let rowsAffected = sqlCmd.ExecuteNonQuery()
    match rowsAffected with 
    | 1 -> Success (Some customer) //todo: instead of Success customer, this should be Success "insert" or Success "update"
    | _ -> Failure (String.Format("upserting WC Customer - 0 rows affected for CustomerNumber {0}", customer.CustomerNumber))
    