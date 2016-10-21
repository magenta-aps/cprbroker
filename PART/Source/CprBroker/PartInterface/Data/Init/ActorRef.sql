/****** Object:  Table [dbo].[ActorRef]    Script Date: 11/21/2013 10:16:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ActorRef]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ActorRef](
	[ActorRefId] [uniqueidentifier] NOT NULL,
	[Type] [int] NOT NULL,
	[Value] [varchar](50) NULL,
 CONSTRAINT [PK_ActorRef] PRIMARY KEY CLUSTERED 
(
	[ActorRefId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

/****** Object:  Default [DF_ActorRef_ActorRefId]    Script Date: 11/21/2013 10:16:51 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_ActorRef_ActorRefId]') AND parent_object_id = OBJECT_ID(N'[dbo].[ActorRef]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ActorRef_ActorRefId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ActorRef] ADD  CONSTRAINT [DF_ActorRef_ActorRefId]  DEFAULT (newid()) FOR [ActorRefId]
END


End
GO
