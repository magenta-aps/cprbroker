IF NOT EXISTS (SELECT * FROM sys.tables WHERE name=N'Operation')
BEGIN
	CREATE TABLE Operation(
		OperationId UNIQUEIDENTIFIER,
		ActivityId UNIQUEIDENTIFIER,
		OperationTypeId INT,
		OperationKey VARCHAR(250),
		CONSTRAINT PK_Operation PRIMARY KEY NONCLUSTERED (OperationId),
		CONSTRAINT FK_Operation_OperationType FOREIGN KEY (OperationTypeId) REFERENCES OperationType (OperationTypeId),
		CONSTRAINT FK_Operation_Activity      FOREIGN KEY (ActivityId)      REFERENCES Activity (ActivityId)
	)ON [PRIMARY] 
END
GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'Operation') AND name = N'IX_Operation_OperationKey')
BEGIN
	CREATE CLUSTERED INDEX IX_Operation_OperationKey ON Operation(
		OperationKey ASC
	)
END
GO
