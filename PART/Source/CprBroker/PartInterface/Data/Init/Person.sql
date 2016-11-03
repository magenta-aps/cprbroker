/****** Object:  Table [dbo].[Person]    Script Date: 11/21/2013 10:16:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Person]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Person](
	[UUID] [uniqueidentifier] NOT NULL,
	[UserInterfaceKeyText] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Object] PRIMARY KEY CLUSTERED 
(
	[UUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Default [DF_Person_UUID]    Script Date: 11/21/2013 10:16:51 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_Person_UUID]') AND parent_object_id = OBJECT_ID(N'[dbo].[Person]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Person_UUID]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Person] ADD  CONSTRAINT [DF_Person_UUID]  DEFAULT (newid()) FOR [UUID]
END


End
GO
