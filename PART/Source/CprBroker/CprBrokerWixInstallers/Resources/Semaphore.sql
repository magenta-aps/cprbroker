IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Semaphore]') AND type in (N'U'))
BEGIN    
    CREATE TABLE [dbo].[Semaphore](
	    [SemaphoreId] [uniqueidentifier] NOT NULL 
            CONSTRAINT [PK_Semaphore] PRIMARY KEY CLUSTERED ([SemaphoreId] ASC)
            CONSTRAINT [DF_Semaphore_SemaphoreId]  DEFAULT (newid()),
	    [CreatedDate] [datetime] NOT NULL,
		[WaitCount] INT DEFAULT 1 NULL,
	    [SignaledDate] [datetime] NULL,
    ) ON [PRIMARY]
END
GO