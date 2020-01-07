module InputService

open GlobalTypes
open Model

let loadCustomers () =
        Failure [
            {CustomerNumber="abc123"; CompanyCode="CCabc" }; 
            {CustomerNumber="abc456"; CompanyCode="CCdef" }
        ]