/****** Object:  Table [dbo].[BirthdateSubscription] ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BirthdateSubscription]') AND type in (N'U'))
DROP TABLE [dbo].[BirthdateSubscription]
GO

/****** Object:  Table [dbo].[BirthdateSubscription] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BirthdateSubscription]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[BirthdateSubscription](
	[SubscriptionId] [uniqueidentifier] NOT NULL,
	[AgeYears] [int] NULL,
	[PriorDays] [int] NOT NULL DEFAULT 0,
    CONSTRAINT [PK_BirthdateSubscription] PRIMARY KEY CLUSTERED ([SubscriptionId] ASC) ,
    CONSTRAINT [FK_BirthdateSubscription_Subscription] FOREIGN KEY([SubscriptionId]) REFERENCES [dbo].[Subscription] ([SubscriptionId]) ON UPDATE CASCADE ON DELETE CASCADE
)
END
GO
