/****** Object:  Table [dbo].[DataSubscription] ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DataSubscription]') AND type in (N'U'))
DROP TABLE [dbo].[DataSubscription]
GO

/****** Object:  Table [dbo].[DataSubscription] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DataSubscription]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DataSubscription](
	[SubscriptionId] [uniqueidentifier] NOT NULL,
    CONSTRAINT [PK_DataSubscription] PRIMARY KEY CLUSTERED ([SubscriptionId] ASC),
    CONSTRAINT [FK_DataSubscription_Subscription] FOREIGN KEY([SubscriptionId]) REFERENCES [dbo].[Subscription] ([SubscriptionId]) ON UPDATE CASCADE ON DELETE CASCADE
)
END
GO
