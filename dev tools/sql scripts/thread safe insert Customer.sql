use WeConnectSales_Monkey

select * 
from Customers c
where c.CustomerNumber like 'CN%'
order by c.Id desc

insert into Customers(CustomerNumber, CompanyCode, [Name], LanguageCode, IsActive)
(select 'CN-TS', 'CC01', 'thread safe test', 'NL', 1
where not exists (select * from Customers c where c.Id = 50133))
SELECT @@ROWCOUNT