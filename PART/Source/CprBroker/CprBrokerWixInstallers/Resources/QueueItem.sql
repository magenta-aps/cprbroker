IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueueItem]') AND type in (N'U'))
    DROP TABLE [dbo].[QueueItem]
GO

CREATE TABLE [dbo].[QueueItem](
	[QueueItemId] [int] IDENTITY(1,1) NOT NULL,
	[QueueId] [uniqueidentifier] NOT NULL
        CONSTRAINT [FK_QueueItem_Queue] FOREIGN KEY([QueueId]) REFERENCES [dbo].[Queue] ([QueueId]) ON UPDATE CASCADE ON DELETE CASCADE,
	[ItemKey] [varchar](50) NOT NULL,
	[CreatedTS] [datetime] NOT NULL
        CONSTRAINT [DF_QueueItem_CreatedTS]  DEFAULT (getdate()) ,
	[AttemptCount] [int] NOT NULL
        CONSTRAINT [DF_QueueItem_AttemptCount]  DEFAULT ((0)) ,
    CONSTRAINT [PK_QueueItem] PRIMARY KEY CLUSTERED ([QueueItemId] ASC)
    
) ON [PRIMARY]

GO
