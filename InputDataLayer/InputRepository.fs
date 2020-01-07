module InputRepository

open System
open System.Data.SqlClient

open GlobalTypes
open Model

let loadCustomersHelper () = 
    let query = @"select * 
from CustomerBasic cb 
join CustomerCompany cc on cc.CustomerNumber = cb.CustomerNumber"
    
    //todo: reusable method for command and connection
    use sqlConn = new SqlConnection("Server=AMSAPP012\Dev;Initial Catalog=SAPImport_Monkey;Trusted_Connection=true;") //todo: connection string in app.config
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
    Success (Seq.toList records) //todo: does Seq.toList create a new list?



let loadCustomers () =
    try
        loadCustomersHelper()
    with
        | :? Exception as ex -> String.Format("Exception.{2}Message:{0},{2}Stack Trace:{1}", ex.StackTrace, ex.Message, Environment.NewLine) |> Failure 