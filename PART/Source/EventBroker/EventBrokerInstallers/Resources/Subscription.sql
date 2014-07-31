/****** Object:  Table [dbo].[Subscription] ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Subscription]') AND type in (N'U'))
    DROP TABLE [dbo].[Subscription]
GO

/****** Object:  Table [dbo].[Subscription] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Subscription]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Subscription](
	[SubscriptionId] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[SubscriptionTypeId] [int] NOT NULL,
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[IsForAllPersons] [bit] NOT NULL DEFAULT 1,
	[Criteria] [XML] NULL,
	[LastCheckedUUID] [uniqueidentifier] NULL,
	[Created] [Datetime] NOT NULL,
	[Deactivated] [Datetime] NULL,
    [Ready] BIT NOT NULL DEFAULT 0
    CONSTRAINT [PK_Subscription] PRIMARY KEY CLUSTERED ([SubscriptionId] ASC),
    CONSTRAINT [FK_Subscription_SubscriptionType] FOREIGN KEY([SubscriptionTypeId]) REFERENCES [dbo].[SubscriptionType] ([SubscriptionTypeId])
)
END
GO
