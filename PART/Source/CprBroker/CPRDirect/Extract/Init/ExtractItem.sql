/****** Object:  Table [dbo].[ExtractItem]    Script Date: 11/21/2013 10:16:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ExtractItem]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ExtractItem](
	[ExtractItemId] [uniqueidentifier] NOT NULL,
	[ExtractId] [uniqueidentifier] NOT NULL,
	[PNR] [varchar](10) NOT NULL,
	[RelationPNR] [varchar](10) NULL,
	[RelationPNR2] [varchar](10) NULL,
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

/****** Object:  Default [DF_ExtractItem_ExtractItemId]    Script Date: 11/21/2013 10:16:51 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_ExtractItem_ExtractItemId]') AND parent_object_id = OBJECT_ID(N'[dbo].[ExtractItem]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ExtractItem_ExtractItemId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ExtractItem] ADD  CONSTRAINT [DF_ExtractItem_ExtractItemId]  DEFAULT (newid()) FOR [ExtractItemId]
END


End
GO

/****** Object:  ForeignKey [FK_ExtractItem_Extract]    Script Date: 11/21/2013 10:16:51 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ExtractItem_Extract]') AND parent_object_id = OBJECT_ID(N'[dbo].[ExtractItem]'))
ALTER TABLE [dbo].[ExtractItem]  WITH NOCHECK ADD  CONSTRAINT [FK_ExtractItem_Extract] FOREIGN KEY([ExtractId])
REFERENCES [dbo].[Extract] ([ExtractId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ExtractItem_Extract]') AND parent_object_id = OBJECT_ID(N'[dbo].[ExtractItem]'))
ALTER TABLE [dbo].[ExtractItem] CHECK CONSTRAINT [FK_ExtractItem_Extract]
GO
