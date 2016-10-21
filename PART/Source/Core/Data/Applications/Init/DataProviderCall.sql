IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DataProviderCall]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[DataProviderCall](
	    [DataProviderCallId] [uniqueidentifier] NOT NULL
            CONSTRAINT [DF_DataProviderCall_DataProviderCallId]  DEFAULT (newid())
            CONSTRAINT [PK_DataProviderCall] PRIMARY KEY NONCLUSTERED ([DataProviderCallId] ASC),
	    [ActivityId] [uniqueidentifier] NOT NULL,
	    [CallTime] [datetime] NOT NULL
            CONSTRAINT [DF_DataProviderCall_CallTime]  DEFAULT (getdate()),
	    [DataProviderType] [varchar](250) NOT NULL,
	    [Cost] [decimal](18, 4) NOT NULL,
	    [Operation] [varchar](250) NOT NULL,
	    [Input] [varchar](250) NOT NULL,
	    [Success] [bit] NULL
    ) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[DataProviderCall]') AND name = N'IX_DataProviderCall_CallTime')
    CREATE CLUSTERED INDEX [IX_DataProviderCall_CallTime] ON [dbo].[DataProviderCall] ([CallTime] ASC)
GO

