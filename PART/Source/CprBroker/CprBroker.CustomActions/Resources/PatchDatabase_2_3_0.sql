IF NOT EXISTS (SELECT * FROM dbo.OperationType WHERE OperationTypeId = 12)
	INSERT INTO dbo.OperationType (Name, OperationTypeId) 
	VALUES('RemovePerson',12)