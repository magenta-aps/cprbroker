IF NOT EXISTS (SELECT * FROM dbo.OperationType WHERE OperationTypeId = 12)
	INSERT INTO dbo.OperationType (Name, OperationTypeId) 
	VALUES('RemovePerson',12)

GO

If NOT EXISTS (SELECT * FROM dbo.[Queue] WHERE TypeName like 'CprBroker.PartInterface.Tracking.CleanupQueue%')
	INSERT INTO dbo.[Queue] (TypeId,TypeName,BatchSize,MaxRetry)
	VALUES(400,'CprBroker.PartInterface.Tracking.CleanupQueue, CprBroker.PartInterface.Tracking', 100, 100)