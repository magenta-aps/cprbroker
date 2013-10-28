/****** Object:  Table [dbo].[Extract].[Ready]     ******/
IF NOT EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Extract]') AND name = 'Ready')
BEGIN
	ALTER TABLE dbo.Extract ADD
		Ready bit NOT NULL CONSTRAINT DF_Extract_Ready DEFAULT 0 
END
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

/****** Object:  Table [dbo].[ExtractPersonStaging]    Script Date: 10/17/2012 19:15:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ExtractPersonStaging]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ExtractPersonStaging](
	[ExtractPersonStagingId] [uniqueidentifier] NOT NULL,
	[ExtractId] [uniqueidentifier] NOT NULL,
	[PNR] [varchar](10) NOT NULL,
 CONSTRAINT [PK_ExtractPersonStaging] PRIMARY KEY CLUSTERED 
(
	[ExtractPersonStagingId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING ON
GO
/****** Object:  Default [DF_ExtractPersonStaging_ExtractPersonStagingId]    Script Date: 10/17/2012 19:15:13 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_ExtractPersonStaging_ExtractPersonStagingId]') AND parent_object_id = OBJECT_ID(N'[dbo].[ExtractPersonStaging]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ExtractPersonStaging_ExtractPersonStagingId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ExtractPersonStaging] ADD  CONSTRAINT [DF_ExtractPersonStaging_ExtractPersonStagingId]  DEFAULT (newid()) FOR [ExtractPersonStagingId]
END


End
GO
/****** Object:  ForeignKey [FK_ExtractPersonStaging_Extract]    Script Date: 10/17/2012 19:15:13 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ExtractPersonStaging_Extract]') AND parent_object_id = OBJECT_ID(N'[dbo].[ExtractPersonStaging]'))
ALTER TABLE [dbo].[ExtractPersonStaging]  WITH CHECK ADD  CONSTRAINT [FK_ExtractPersonStaging_Extract] FOREIGN KEY([ExtractId])
REFERENCES [dbo].[Extract] ([ExtractId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ExtractPersonStaging_Extract]') AND parent_object_id = OBJECT_ID(N'[dbo].[ExtractPersonStaging]'))
ALTER TABLE [dbo].[ExtractPersonStaging] CHECK CONSTRAINT [FK_ExtractPersonStaging_Extract]
GO
