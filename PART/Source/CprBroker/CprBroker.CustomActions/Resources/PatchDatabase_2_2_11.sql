IF NOT EXISTS (SELECT * FROM OperationType WHERE OperationTypeId = 11)
	INSERT INTO OperationType(Name, OperationTypeId) VALUES ('DprDiversion',11)