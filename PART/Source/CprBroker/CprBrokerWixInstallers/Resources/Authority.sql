
/****** Object:  Table [dbo].[Authority]    Script Date: 11/21/2013 10:16:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Authority]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Authority](
	[AuthorityCode] [varchar](4) NOT NULL
		CONSTRAINT [PK_Authority] PRIMARY KEY CLUSTERED (	[AuthorityCode] ASC),
	[AuthorityType] [varchar](2) NOT NULL,
	[AuthorityGroup] [char](10) NOT NULL,
	[UpdateTime] [datetime] NOT NULL,
	[AuthorityPhone] [varchar](8) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NULL,
	[AuthorityName] [nvarchar](20) NULL,
	[Address] [nvarchar](34) NULL,
	[AddressLine1] [nvarchar](34) NULL,
	[AddressLine2] [nvarchar](34) NULL,
	[AddressLine3] [nvarchar](34) NULL,
	[AddressLine4] [nvarchar](34) NULL,
	[Telefax] [varchar](8) NULL,
	[FullName] [nvarchar](60) NULL,
	[Email] [nvarchar](100) NULL,
	[Alpha2CountryCode] [char](2) NOT NULL,
	[Alpha3CountryCode] [char](3) NOT NULL,
	[NumericCountryCode] [char](3) NOT NULL, 
 ) ON [PRIMARY]
END
GO