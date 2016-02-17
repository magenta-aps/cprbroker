IF NOT EXISTS (SELECT * FROM sys.indexes si WHERE si.name = N'IX_QueueItem_QueueId')
	CREATE NONCLUSTERED INDEX [IX_QueueItem_QueueId] ON [dbo].[QueueItem] 
	(
		[QueueId] ASC
	)
	INCLUDE ( [AttemptCount],[SemaphoreId]) 
GO
