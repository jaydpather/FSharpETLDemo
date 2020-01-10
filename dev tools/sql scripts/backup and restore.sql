backup database SAPImport_Monkey 
	to disk = 'D:\db-backup\SAPImport_Monkey_2020-01-10.bak'
GO

restore database SAPImport_RD  
	from disk = 'D:\db-backup\SAPImport_Monkey_2020-01-10.bak'
	with move 'SAPImport_Monkey' to 'K:\Data\SAPImport_RD.mdf',
	move 'SAPImport_Monkey_Log' to 'L:\Log\SAPImport_RD_log.ldf'


backup database WeConnectSales_Monkey
	to disk = 'D:\db-backup\WeConnectSales_Monkey_2020-01-10.bak'
GO

restore database WeConnectSales_RD  
	from disk = 'D:\db-backup\WeConnectSales_Monkey_2020-01-10.bak'
	with move 'WeConnectSales_Monkey' to 'K:\Data\WeConnectSales_Monkey_RD.mdf',
	move 'WeConnectSales_Monkey_log' to 'L:\Log\WeConnectSales_RD_log.ldf'
