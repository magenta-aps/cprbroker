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
