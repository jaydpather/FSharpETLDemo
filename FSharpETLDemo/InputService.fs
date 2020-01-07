module InputService

open GlobalTypes
open Model

let loadCustomers ():SAPCustomer list =
        //Success 
        [
            {CustomerNumber="abc123"; CompanyCode="CCabc" }; 
            {CustomerNumber="abc456"; CompanyCode="CCdef" }
        ]