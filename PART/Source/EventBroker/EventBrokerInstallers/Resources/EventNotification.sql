
/****** Object:  ForeignKey [FK_EventNotification_Subscription]    Script Date: 02/13/2011 17:59:04 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_EventNotification_Subscription]') AND parent_object_id = OBJECT_ID(N'[dbo].[EventNotification]'))
ALTER TABLE [dbo].[EventNotification] DROP CONSTRAINT [FK_EventNotification_Subscription]
GO
/****** Object:  Default [DF_EventNotification_EventNotificationId]    Script Date: 02/13/2011 17:59:04 ******/
IF  EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_EventNotification_EventNotificationId]') AND parent_object_id = OBJECT_ID(N'[dbo].[EventNotification]'))
Begin
ALTER TABLE [dbo].[EventNotification] DROP CONSTRAINT [DF_EventNotification_EventNotificationId]

End
GO
/****** Object:  Table [dbo].[EventNotification]    Script Date: 02/13/2011 17:59:04 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EventNotification]') AND type in (N'U'))
DROP TABLE [dbo].[EventNotification]
GO
/****** Object:  Table [dbo].[EventNotification]    Script Date: 02/13/2011 17:59:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EventNotification]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[EventNotification](
	[EventNotificationId] [uniqueidentifier] NOT NULL,
	[SubscriptionId] [uniqueidentifier] NOT NULL,
	[PersonUuid] [uniqueidentifier] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[NotificationDate] [datetime] NULL,
	[Succeeded] [bit] NULL,
	[IsLastNotification] [bit] NULL,
 CONSTRAINT [PK_EventNotification] PRIMARY KEY CLUSTERED 
(
	[EventNotificationId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
/****** Object:  Default [DF_EventNotification_EventNotificationId]    Script Date: 02/13/2011 17:59:04 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_EventNotification_EventNotificationId]') AND parent_object_id = OBJECT_ID(N'[dbo].[EventNotification]'))
Begin
ALTER TABLE [dbo].[EventNotification] ADD  CONSTRAINT [DF_EventNotification_EventNotificationId]  DEFAULT (newid()) FOR [EventNotificationId]

End
GO
/****** Object:  ForeignKey [FK_EventNotification_Subscription]    Script Date: 02/13/2011 17:59:04 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_EventNotification_Subscription]') AND parent_object_id = OBJECT_ID(N'[dbo].[EventNotification]'))
ALTER TABLE [dbo].[EventNotification]  WITH CHECK ADD  CONSTRAINT [FK_EventNotification_Subscription] FOREIGN KEY([SubscriptionId])
REFERENCES [dbo].[Subscription] ([SubscriptionId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EventNotification] CHECK CONSTRAINT [FK_EventNotification_Subscription]
GO