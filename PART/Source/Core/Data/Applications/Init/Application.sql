/****** Object:  Table [dbo].[Application]    Script Date: 11/21/2013 10:16:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Application]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Application](
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Token] [varchar](50) NOT NULL,
	[RegistrationDate] [datetime] NOT NULL,
	[IsApproved] [bit] NOT NULL,
	[ApprovedDate] [datetime] NULL,
 CONSTRAINT [PK_Application] PRIMARY KEY CLUSTERED 
(
	[ApplicationId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Default [DF_Application_ApplicationId]    Script Date: 11/21/2013 10:16:51 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_Application_ApplicationId]') AND parent_object_id = OBJECT_ID(N'[dbo].[Application]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Application_ApplicationId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Application] ADD  CONSTRAINT [DF_Application_ApplicationId]  DEFAULT (newid()) FOR [ApplicationId]
END


End
GO
/****** Object:  Default [DF_Application_IsApproved]    Script Date: 11/21/2013 10:16:51 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_Application_IsApproved]') AND parent_object_id = OBJECT_ID(N'[dbo].[Application]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Application_IsApproved]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Application] ADD  CONSTRAINT [DF_Application_IsApproved]  DEFAULT ((0)) FOR [IsApproved]
END


End
GO
