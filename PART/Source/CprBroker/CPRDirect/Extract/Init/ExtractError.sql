/****** Object:  Table [dbo].[ExtractError]    Script Date: 11/21/2013 10:16:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ExtractError]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ExtractError](
	[ExtractErrorId] [uniqueidentifier] NOT NULL,
	[ExtractId] [uniqueidentifier] NOT NULL,
	[Contents] [nvarchar](157) NOT NULL,
 CONSTRAINT [PK_ExtractError] PRIMARY KEY CLUSTERED 
(
	[ExtractErrorId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

/****** Object:  Default [DF__ExtractEr__Extra__75586032]    Script Date: 11/21/2013 10:16:51 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF__ExtractEr__Extra__75586032]') AND parent_object_id = OBJECT_ID(N'[dbo].[ExtractError]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__ExtractEr__Extra__75586032]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ExtractError] ADD  DEFAULT (newid()) FOR [ExtractErrorId]
END


End
GO

/****** Object:  ForeignKey [FK_ExtractError_Extract]    Script Date: 11/21/2013 10:16:51 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ExtractError_Extract]') AND parent_object_id = OBJECT_ID(N'[dbo].[ExtractError]'))
ALTER TABLE [dbo].[ExtractError]  WITH CHECK ADD  CONSTRAINT [FK_ExtractError_Extract] FOREIGN KEY([ExtractId])
REFERENCES [dbo].[Extract] ([ExtractId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ExtractError_Extract]') AND parent_object_id = OBJECT_ID(N'[dbo].[ExtractError]'))
ALTER TABLE [dbo].[ExtractError] CHECK CONSTRAINT [FK_ExtractError_Extract]
GO
