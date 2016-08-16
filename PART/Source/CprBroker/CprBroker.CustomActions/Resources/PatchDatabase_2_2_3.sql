-- If the new columns are not there, drop the cache table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('PersonSearchCache') AND name = 'MunicipalityCode' )
BEGIN
    -- Only if the table exists
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'PersonSearchCache')
        DROP TABLE PersonSearchCache
END

GO

/*
   ==============================
   ====      Semaphores      ====
   ==============================
 */
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Semaphore]') AND type in (N'U'))
BEGIN    
    CREATE TABLE [dbo].[Semaphore](
	    [SemaphoreId] [uniqueidentifier] NOT NULL 
            CONSTRAINT [PK_Semaphore] PRIMARY KEY CLUSTERED ([SemaphoreId] ASC)
            CONSTRAINT [DF_Semaphore_SemaphoreId]  DEFAULT (newid()),
	    [CreatedDate] [datetime] NOT NULL,
	    [SignaledDate] [datetime] NULL,
    ) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Queue]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Queue](
	    [QueueId] [uniqueidentifier] NOT NULL            
            CONSTRAINT [PK_Queue] PRIMARY KEY CLUSTERED ([QueueId] ASC)
            CONSTRAINT [DF_Queue_QueueId] DEFAULT NEWID(),
	    [TypeId] [int] NULL,
	    [TypeName] [varchar](250) NOT NULL,
	    [BatchSize] [int] NOT NULL,
	    [MaxRetry] [int] NOT NULL,
	    [EncryptedData] [varbinary](max) NULL
    ) ON [PRIMARY]
END
GO

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
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'SemaphoreId' AND object_id = object_id(N'Extract'))
BEGIN
    ALTER TABLE [dbo].[Extract] ADD [SemaphoreId] UNIQUEIDENTIFIER CONSTRAINT [FK_Extract_Semaphore] FOREIGN KEY (SemaphoreId) REFERENCES [dbo].[Semaphore] ([SemaphoreId])
END
GO