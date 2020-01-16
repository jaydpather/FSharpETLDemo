use SAPImport_RD

--select * from CustomerBasic
--select * from CustomerCompany where ImportStatusId = 3

delete from CustomerCompany
delete from CustomerBasic

insert into CustomerBasic
values ('CN98765432', 'DE', 'test import 2', 'Cologne', '9074AB', 'UH', 'DE', '111246', '145', '1487223314', getdate(), null, 'ABCD')

insert into CustomerCompany
values('CN98765432', 'W031', getdate(), null, 3) --todo: won't need ImportStatusId once it's moved to a new table


