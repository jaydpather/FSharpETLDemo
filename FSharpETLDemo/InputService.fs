module InputService

open Microsoft.EntityFrameworkCore
open Microsoft.EntityFrameworkCore.Query


open System.Data.SqlClient


open GlobalTypes
open Model

type SAPImportDBContext(connStrSettingName) = 
    inherit DbContext(connStrSettingName)

    [<DefaultValue>]
    val mutable customers:DbSet<SAPCustomer>

    member x.Customers
        with get() = x.customers
        and set v = x.customers <- v

let loadCustomers () = 
    let query = @"select * 
from CustomerBasic cb 
join CustomerCompany cc on cc.CustomerNumber = cb.CustomerNumber"
    
    //todo: reusable method for command and connection
    use sqlConn = new SqlConnection("Server=AMSAPP012\Dev;Initial Catalog=SAPImport_Monkey;Trusted_Connection=true;")
    sqlConn.Open()
    use sqlCmd = new SqlCommand(query, sqlConn) //todo: does F# have String.Empty?
    use dataReader = sqlCmd.ExecuteReader();
    
    let records = seq {//todo: what is the difference between Seq and List?
        while(dataReader.Read()) do //todo: seems like you have to use a while loop here, b/c that's how DataReader works. is there an F# data reader?
        
            yield {
                CustomerNumber=(string)dataReader.["CustomerNumber"]; 
                CompanyCode=(string)dataReader.["CompanyCode"]
            }
    }
    Failure (Seq.toList records) //todo: does Seq.toList create a new list?

