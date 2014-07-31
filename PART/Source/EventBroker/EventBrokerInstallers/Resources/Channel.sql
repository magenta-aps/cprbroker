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
	[ChannelId] [uniqueidentifier] NOT NULL DEFAULT newid(),
	[ChannelTypeId] [int] NOT NULL,
	[SubscriptionId] [uniqueidentifier] NOT NULL,
	[Url] [varchar](250) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    CONSTRAINT [PK_Channel] PRIMARY KEY CLUSTERED ([ChannelId] ASC) ,
    CONSTRAINT [IX_Channel] UNIQUE NONCLUSTERED ([SubscriptionId] ASC),    
    CONSTRAINT [FK_Channel_ChannelType] FOREIGN KEY([ChannelTypeId]) REFERENCES [dbo].[ChannelType] ([ChannelTypeId]),
    CONSTRAINT [FK_Channel_Subscription] FOREIGN KEY([SubscriptionId]) REFERENCES [dbo].[Subscription] ([SubscriptionId]) ON UPDATE CASCADE ON DELETE CASCADE
)
END
GO
