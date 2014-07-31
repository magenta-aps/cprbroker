
/****** Object:  Table [dbo].[EventNotification] ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EventNotification]') AND type in (N'U'))
DROP TABLE [dbo].[EventNotification]
GO

/****** Object:  Table [dbo].[EventNotification] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EventNotification]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[EventNotification](
	[EventNotificationId] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[SubscriptionId] [uniqueidentifier] NOT NULL,
	[PersonUuid] [uniqueidentifier] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[NotificationDate] [datetime] NULL,
	[Succeeded] [bit] NULL,
	[IsLastNotification] [bit] NULL,
    CONSTRAINT [PK_EventNotification] PRIMARY KEY CLUSTERED ([EventNotificationId] ASC),
    CONSTRAINT [FK_EventNotification_Subscription] FOREIGN KEY([SubscriptionId]) REFERENCES [dbo].[Subscription] ([SubscriptionId]) ON UPDATE CASCADE ON DELETE CASCADE
)
END
GO