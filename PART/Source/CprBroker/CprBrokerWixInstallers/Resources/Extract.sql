IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Extract]') AND type in (N'U'))
    DROP TABLE [dbo].[Extract]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Extract](
	[ExtractId] [uniqueidentifier] NOT NULL 
        CONSTRAINT [DF_Extract_ExtractId] DEFAULT NEWID(),
	[Filename] [nvarchar](max) NOT NULL,
	[ExtractDate] [datetime] NOT NULL,
	[ImportDate] [datetime] NOT NULL,
	[StartRecord] [nvarchar](max) NOT NULL,
	[EndRecord] [nvarchar](max) NOT NULL,
	[Ready] [bit] NOT NULL 
        CONSTRAINT [DF_Extract_Ready] DEFAULT ((0)),
	[ProcessedLines] [bigint] NULL,
    CONSTRAINT [PK_Extract] PRIMARY KEY CLUSTERED (	[ExtractId] ASC),
    
) ON [PRIMARY]
GO
