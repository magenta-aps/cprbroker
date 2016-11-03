IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BirthdateEventNotification]') AND type in (N'U'))
DROP TABLE [dbo].[BirthdateEventNotification]
GO

/****** Object:  Table [dbo].[BirthdateEventNotification]    Script Date: 02/13/2011 17:59:04 ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BirthdateEventNotification]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[BirthdateEventNotification](
	[EventNotificationId] [uniqueidentifier] NOT NULL,
	[Age] [int] NOT NULL,
    CONSTRAINT [PK_BirthdateEventNotification] PRIMARY KEY CLUSTERED ([EventNotificationId] ASC),
    CONSTRAINT [FK_BirthdateEventNotification_EventNotification] FOREIGN KEY ([EventNotificationId]) REFERENCES [dbo].[EventNotification] ([EventNotificationId])
)
END
GO