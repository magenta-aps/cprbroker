IF NOT EXISTS (SELECT * FROM sys.tables WHERE name=N'OperationType')
BEGIN
	CREATE TABLE OperationType(
		OperationTypeId INT,
		Name VARCHAR(250),
		CONSTRAINT PK_OperationType PRIMARY KEY (OperationTypeId)
	)ON [PRIMARY] 
END
GO