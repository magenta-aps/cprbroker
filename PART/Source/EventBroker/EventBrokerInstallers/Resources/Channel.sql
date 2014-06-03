/****** Object:  ForeignKey [FK_Channel_ChannelType]    Script Date: 02/13/2011 17:59:04 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Channel_ChannelType]') AND parent_object_id = OBJECT_ID(N'[dbo].[Channel]'))
ALTER TABLE [dbo].[Channel] DROP CONSTRAINT [FK_Channel_ChannelType]
GO
/****** Object:  ForeignKey [FK_Channel_Subscription]    Script Date: 02/13/2011 17:59:04 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Channel_Subscription]') AND parent_object_id = OBJECT_ID(N'[dbo].[Channel]'))
ALTER TABLE [dbo].[Channel] DROP CONSTRAINT [FK_Channel_Subscription]
GO
/****** Object:  Default [DF_Channel_ChannelId]    Script Date: 02/13/2011 17:59:04 ******/
IF  EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_Channel_ChannelId]') AND parent_object_id = OBJECT_ID(N'[dbo].[Channel]'))
Begin
ALTER TABLE [dbo].[Channel] DROP CONSTRAINT [DF_Channel_ChannelId]

End
GO
/****** Object:  Table [dbo].[Channel]    Script Date: 02/13/2011 17:59:04 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Channel]') AND type in (N'U'))
DROP TABLE [dbo].[Channel]
GO
/****** Object:  Table [dbo].[Channel]    Script Date: 02/13/2011 17:59:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Channel]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Channel](
	[ChannelId] [uniqueidentifier] NOT NULL,
	[ChannelTypeId] [int] NOT NULL,
	[SubscriptionId] [uniqueidentifier] NOT NULL,
	[Url] [varchar](250) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_Channel] PRIMARY KEY CLUSTERED 
(
	[ChannelId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF),
 CONSTRAINT [IX_Channel] UNIQUE NONCLUSTERED 
(
	[SubscriptionId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
/****** Object:  Default [DF_Channel_ChannelId]    Script Date: 02/13/2011 17:59:04 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_Channel_ChannelId]') AND parent_object_id = OBJECT_ID(N'[dbo].[Channel]'))
Begin
ALTER TABLE [dbo].[Channel] ADD  CONSTRAINT [DF_Channel_ChannelId]  DEFAULT (newid()) FOR [ChannelId]

End
GO
/****** Object:  ForeignKey [FK_Channel_ChannelType]    Script Date: 02/13/2011 17:59:04 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Channel_ChannelType]') AND parent_object_id = OBJECT_ID(N'[dbo].[Channel]'))
ALTER TABLE [dbo].[Channel]  WITH CHECK ADD  CONSTRAINT [FK_Channel_ChannelType] FOREIGN KEY([ChannelTypeId])
REFERENCES [dbo].[ChannelType] ([ChannelTypeId])
GO
ALTER TABLE [dbo].[Channel] CHECK CONSTRAINT [FK_Channel_ChannelType]
GO
/****** Object:  ForeignKey [FK_Channel_Subscription]    Script Date: 02/13/2011 17:59:04 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Channel_Subscription]') AND parent_object_id = OBJECT_ID(N'[dbo].[Channel]'))
ALTER TABLE [dbo].[Channel]  WITH CHECK ADD  CONSTRAINT [FK_Channel_Subscription] FOREIGN KEY([SubscriptionId])
REFERENCES [dbo].[Subscription] ([SubscriptionId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Channel] CHECK CONSTRAINT [FK_Channel_Subscription]
GO