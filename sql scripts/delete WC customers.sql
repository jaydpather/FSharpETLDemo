use WeConnectSales_Monkey

select * 
from Customers c
where c.CustomerNumber like 'CN%'

begin tran
	delete
	from Customers
	where CustomerNumber like 'CN%'
rollback tran
commit tran
