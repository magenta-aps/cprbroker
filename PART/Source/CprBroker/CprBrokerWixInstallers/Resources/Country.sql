
/****** Object:  Table [dbo].[Country]    Script Date: 11/21/2013 10:16:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Country]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Country](
	[Alpha2Code] [varchar](2) NOT NULL
		CONSTRAINT [PK_Country] PRIMARY KEY CLUSTERED (	[Alpha2Code] ASC),
	[Alpha3Code] [varchar](3) NOT NULL,
	[NumericCode] [int] NOT NULL,
	[CountryName] [nvarchar](60) NOT NULL,
	[Description] [nvarchar](50) NULL,
	[DanishCountryName] [nvarchar](60) NOT NULL,
	[DanishCountryName2] [nvarchar](50) NULL,
	[KmdCode] [int] NULL,
	[KmdCode2] [int] NULL,
	[KmdCode3] [int] NULL,
 
) ON [PRIMARY]
END
GO
