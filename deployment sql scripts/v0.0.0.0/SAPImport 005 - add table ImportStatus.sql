create table ImportStatus
(
	Id int not null primary key identity(1,1),
	StatusName nvarchar(50) not null
)


insert into ImportStatus(StatusName)
values
	('New'),
	('Failed'),
	('In Progress')
