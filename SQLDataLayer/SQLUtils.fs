module SQLUtils

open System.Data.SqlClient

let private executeReader connectionString query dataReaderCallback = 
    use sqlConn = new SqlConnection(connectionString)
    sqlConn.Open()
    use sqlCmd = new SqlCommand(query, sqlConn) //todo*: does F# have String.Empty?
    use dataReader = sqlCmd.ExecuteReader();
    dataReaderCallback dataReader

let getExecuteReaderFunc connectionString = 
    executeReader connectionString


let private getSqlCmd connectionString query callback =
    use sqlConn = new SqlConnection(connectionString)
    sqlConn.Open()
    use sqlCmd = new SqlCommand(query, sqlConn)
    callback sqlCmd

let getSqlCmdFunc connectionString = 
    getSqlCmd connectionString