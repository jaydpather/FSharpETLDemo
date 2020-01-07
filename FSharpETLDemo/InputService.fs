module InputService

open GlobalTypes
open Model

let loadCustomers () =
        Success [
            {CustomerNumber="abc123"; CompanyCode="CCabc" }; 
            {CustomerNumber="abc456"; CompanyCode="CCdef" }
        ]