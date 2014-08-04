IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Queue]') AND type in (N'U'))
    DROP TABLE [dbo].[Queue]
GO

CREATE TABLE [dbo].[Queue](
	[QueueId] [uniqueidentifier] NOT NULL
        CONSTRAINT [DF_Queue_QueueId] DEFAULT NEWID(),
	[TypeId] [int] NULL,
	[TypeName] [varchar](250) NOT NULL,
	[BatchSize] [int] NOT NULL,
	[MaxRetry] [int] NOT NULL,
	[EncryptedData] [varbinary](max) NULL,
    CONSTRAINT [PK_Queue] PRIMARY KEY CLUSTERED ([QueueId] ASC)

) ON [PRIMARY]

GO
