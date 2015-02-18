IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueueItem]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[QueueItem](
	    [QueueItemId]   int IDENTITY(1,1)   NOT NULL CONSTRAINT [PK_QueueItem]              PRIMARY KEY CLUSTERED ([QueueItemId] ASC),
	    [QueueId]       uniqueidentifier    NOT NULL CONSTRAINT [FK_QueueItem_Queue]        FOREIGN KEY ([QueueId]) REFERENCES [dbo].[Queue] ([QueueId]) ON UPDATE CASCADE ON DELETE CASCADE,
	    [ItemKey]       varchar(50)         NOT NULL,
	    [CreatedTS]     datetime            NOT NULL CONSTRAINT [DF_QueueItem_CreatedTS]    DEFAULT (getdate()) ,
	    [AttemptCount]  int                 NOT NULL CONSTRAINT [DF_QueueItem_AttemptCount] DEFAULT ((0)),
        [SemaphoreId]   UNIQUEIDENTIFIER             CONSTRAINT [FK_QueueItem_Semaphore]    FOREIGN KEY (SemaphoreId) REFERENCES [dbo].[Semaphore] ([SemaphoreId])
    ) ON [PRIMARY]
END
