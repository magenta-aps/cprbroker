IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DataProvider]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[DataProvider](
	    [DataProviderId] [uniqueidentifier] NOT NULL
            CONSTRAINT [PK_DataProvider] PRIMARY KEY CLUSTERED ([DataProviderId] ASC),
	    [TypeName] [varchar](250) NOT NULL,
	    [Ordinal] [int] NOT NULL,
	    [Data] [image] NULL,
	    [IsExternal] [bit] NOT NULL,
	    [IsEnabled] [bit] NOT NULL
    ) ON [PRIMARY] 
END
GO
