select * 
from CustomerBasic

select * from CustomerCompany

begin tran
	update CustomerBasic
	set [Name] = 'Test import 3',
		City = 'New York'
	where CustomerNumber = 'CN12343567'

	update CustomerCompany
		set CustomerNumber = 'CN12343567'
	where CustomerNumber = 'CN1234356_'

	update CustomerCompany
		set CustomerNumber = 'CN1234356_'
	where CustomerNumber = 'CN12343567'
rollback tran
commit tran