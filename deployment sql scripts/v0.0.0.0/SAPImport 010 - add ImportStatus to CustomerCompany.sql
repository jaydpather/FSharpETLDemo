alter table CustomerCompany
        add ImportStatusId int not null
		constraint fk_CustomerCompany_ImportStatus foreign key(ImportStatusId) REFERENCES ImportStatus(Id)
    default (1)--Optional Default-Constraint.