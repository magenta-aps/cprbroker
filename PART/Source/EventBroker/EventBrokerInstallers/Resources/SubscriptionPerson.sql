/****** Object:  Table [dbo].[SubscriptionPerson] ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SubscriptionPerson]') AND type in (N'U'))
DROP TABLE [dbo].[SubscriptionPerson]
GO

/****** Object:  Table [dbo].[SubscriptionPerson] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SubscriptionPerson]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[SubscriptionPerson](
	[SubscriptionPersonId] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[SubscriptionId] [uniqueidentifier] NULL,
	[PersonUuid] [uniqueidentifier] NULL,
	[Created] [Datetime] NOT NULL,
	[Removed] [Datetime] NULL,
    CONSTRAINT [PK_SubscriptionPerson] PRIMARY KEY CLUSTERED ([SubscriptionPersonId] ASC),
    CONSTRAINT [IX_SubscriptionPerson] UNIQUE NONCLUSTERED ([SubscriptionId] ASC, [PersonUuid] ASC) ,
    CONSTRAINT [FK_SubscriptionPerson_Subscription] FOREIGN KEY([SubscriptionId]) REFERENCES [dbo].[Subscription] ([SubscriptionId]) ON UPDATE CASCADE ON DELETE CASCADE
)
END
GO
