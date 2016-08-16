/*
	This file patches the CPR broker database to version 1.3
	Creates tables Extract, ExtractItem and Authority
	SQL 9.xxx (2005) is a minimum because it makes use of INCLUDE in index for ExtractItem
*/

/****** Object:  Column [dbo].[PersonRegistration].[Table]    Script Date: 23/08/2012 18:36:34 ******/
IF NOT EXISTS(SELECT * FROM sys.columns WHERE object_id= object_id('PersonRegistration') AND name = 'SourceObjects')
BEGIN
	ALTER TABLE [dbo].[PersonRegistration] ADD [SourceObjects] [xml] NULL
END

GO

/****** Object:  Table [dbo].[Extract]    Script Date: 08/08/2012 16:36:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Extract]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Extract](
	[ExtractId] [uniqueidentifier] NOT NULL,
	[Filename] [nvarchar](max) NOT NULL,
	[ExtractDate] [datetime] NOT NULL,
	[ImportDate] [datetime] NOT NULL,
	[StartRecord] [nvarchar](max) NOT NULL,
	[EndRecord] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Extract] PRIMARY KEY CLUSTERED 
(
	[ExtractId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Extract]') AND name = N'IX_Extract_ExtractDate')
CREATE NONCLUSTERED INDEX [IX_Extract_ExtractDate] ON [dbo].[Extract] 
(
	[ExtractDate] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Authority]    Script Date: 08/08/2012 16:36:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Authority]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Authority](
	[AuthorityCode] [varchar](4) NOT NULL,
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
 CONSTRAINT [PK_Authority] PRIMARY KEY CLUSTERED 
(
	[AuthorityCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[ExtractItem]    Script Date: 08/08/2012 16:36:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ExtractItem]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ExtractItem](
	[ExtractItemId] [uniqueidentifier] NOT NULL,
	[ExtractId] [uniqueidentifier] NOT NULL,
	[PNR] [varchar](10) NOT NULL,
	[RelationPNR] [varchar](10) NULL,
	[DataTypeCode] [varchar](10) NOT NULL,
	[Contents] [nvarchar](max) NOT NULL,
 CONSTRAINT [UK_ExtractItem_ExtractItemId] UNIQUE NONCLUSTERED 
(
	[ExtractItemId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ExtractItem]') AND name = N'IX_ExtractItem_PNR_ExtractId')
CREATE CLUSTERED INDEX [IX_ExtractItem_PNR_ExtractId] ON [dbo].[ExtractItem] 
(
	[PNR] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ExtractItem]') AND name = N'IX_ExtractItem_ExtractId')
CREATE NONCLUSTERED INDEX [IX_ExtractItem_ExtractId] ON [dbo].[ExtractItem] 
(
	[ExtractId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ExtractItem]') AND name = N'IX_ExtractItem_RelationPNR')
CREATE NONCLUSTERED INDEX [IX_ExtractItem_RelationPNR] ON [dbo].[ExtractItem] 
(
	[RelationPNR] ASC
)
INCLUDE ( [DataTypeCode]) 
WHERE ([PNR] IS NOT NULL)
WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Default [DF_Extract_ExtractId]    Script Date: 08/08/2012 16:36:34 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_Extract_ExtractId]') AND parent_object_id = OBJECT_ID(N'[dbo].[Extract]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Extract_ExtractId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Extract] ADD  CONSTRAINT [DF_Extract_ExtractId]  DEFAULT (newid()) FOR [ExtractId]
END


End
GO
/****** Object:  Default [DF_ExtractItem_ExtractItemId]    Script Date: 08/08/2012 16:36:34 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_ExtractItem_ExtractItemId]') AND parent_object_id = OBJECT_ID(N'[dbo].[ExtractItem]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ExtractItem_ExtractItemId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ExtractItem] ADD  CONSTRAINT [DF_ExtractItem_ExtractItemId]  DEFAULT (newid()) FOR [ExtractItemId]
END


End
GO
/****** Object:  ForeignKey [FK_ExtractItem_Extract]    Script Date: 08/08/2012 16:36:34 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ExtractItem_Extract]') AND parent_object_id = OBJECT_ID(N'[dbo].[ExtractItem]'))
ALTER TABLE [dbo].[ExtractItem]  WITH NOCHECK ADD  CONSTRAINT [FK_ExtractItem_Extract] FOREIGN KEY([ExtractId])
REFERENCES [dbo].[Extract] ([ExtractId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ExtractItem_Extract]') AND parent_object_id = OBJECT_ID(N'[dbo].[ExtractItem]'))
ALTER TABLE [dbo].[ExtractItem] CHECK CONSTRAINT [FK_ExtractItem_Extract]
GO
