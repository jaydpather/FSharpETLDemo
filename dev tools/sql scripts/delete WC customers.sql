use WeConnectSales_RD

select count(*) from Customers
where CustomerNumber like 'CN%'

delete
from Customers
where CustomerNumber like 'CN%'
