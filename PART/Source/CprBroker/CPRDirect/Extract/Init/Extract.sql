IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Extract]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Extract](
	    [ExtractId] [uniqueidentifier] NOT NULL 
            CONSTRAINT [PK_Extract] PRIMARY KEY CLUSTERED (	[ExtractId] ASC)
            CONSTRAINT [DF_Extract_ExtractId] DEFAULT NEWID(),
	    [Filename] [nvarchar](max) NOT NULL,
	    [ExtractDate] [datetime] NOT NULL,
	    [ImportDate] [datetime] NOT NULL,
	    [StartRecord] [nvarchar](max) NOT NULL,
	    [EndRecord] [nvarchar](max) NOT NULL,
	    [Ready] [bit] NOT NULL  
            CONSTRAINT [DF_Extract_Ready] DEFAULT ((0)),
	    [ProcessedLines] [bigint] NULL,    
        [SemaphoreId]   
            UNIQUEIDENTIFIER CONSTRAINT [FK_Extract_Semaphore] FOREIGN KEY (SemaphoreId) REFERENCES [dbo].[Semaphore] ([SemaphoreId])
    ) ON [PRIMARY]
END

GO
