/****** Object:  ForeignKey [FK_Subscription_SubscriptionType]    Script Date: 02/13/2011 17:59:04 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Subscription_SubscriptionType]') AND parent_object_id = OBJECT_ID(N'[dbo].[Subscription]'))
ALTER TABLE [dbo].[Subscription] DROP CONSTRAINT [FK_Subscription_SubscriptionType]
GO
/****** Object:  Default [DF_Subscription_SubscriptionId]    Script Date: 02/13/2011 17:59:04 ******/
IF  EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_Subscription_SubscriptionId]') AND parent_object_id = OBJECT_ID(N'[dbo].[Subscription]'))
Begin
ALTER TABLE [dbo].[Subscription] DROP CONSTRAINT [DF_Subscription_SubscriptionId]

End
GO
/****** Object:  Default [DF_Subscription_IsForAllPersons]    Script Date: 02/13/2011 17:59:04 ******/
IF  EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_Subscription_IsForAllPersons]') AND parent_object_id = OBJECT_ID(N'[dbo].[Subscription]'))
Begin
ALTER TABLE [dbo].[Subscription] DROP CONSTRAINT [DF_Subscription_IsForAllPersons]

End
GO
/****** Object:  Table [dbo].[Subscription]    Script Date: 02/13/2011 17:59:04 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Subscription]') AND type in (N'U'))
DROP TABLE [dbo].[Subscription]
GO
/****** Object:  Table [dbo].[Subscription]    Script Date: 02/13/2011 17:59:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Subscription]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Subscription](
	[SubscriptionId] [uniqueidentifier] NOT NULL,
	[SubscriptionTypeId] [int] NOT NULL,
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[IsForAllPersons] [bit] NOT NULL,
	[Criteria] [XML] NULL,
	[LastCheckedUUID] [uniqueidentifier] NULL,
	[Created] [Datetime] NOT NULL,
	[Deactivated] [Datetime] NULL,
    [Ready] BIT NOT NULL DEFAULT 0
 CONSTRAINT [PK_Subscription] PRIMARY KEY CLUSTERED 
(
	[SubscriptionId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
/****** Object:  Default [DF_Subscription_SubscriptionId]    Script Date: 02/13/2011 17:59:04 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_Subscription_SubscriptionId]') AND parent_object_id = OBJECT_ID(N'[dbo].[Subscription]'))
Begin
ALTER TABLE [dbo].[Subscription] ADD  CONSTRAINT [DF_Subscription_SubscriptionId]  DEFAULT (newid()) FOR [SubscriptionId]

End
GO
/****** Object:  Default [DF_Subscription_IsForAllPersons]    Script Date: 02/13/2011 17:59:04 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_Subscription_IsForAllPersons]') AND parent_object_id = OBJECT_ID(N'[dbo].[Subscription]'))
Begin
ALTER TABLE [dbo].[Subscription] ADD  CONSTRAINT [DF_Subscription_IsForAllPersons]  DEFAULT ((1)) FOR [IsForAllPersons]

End
GO
/****** Object:  ForeignKey [FK_Subscription_SubscriptionType]    Script Date: 02/13/2011 17:59:04 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Subscription_SubscriptionType]') AND parent_object_id = OBJECT_ID(N'[dbo].[Subscription]'))
ALTER TABLE [dbo].[Subscription]  WITH CHECK ADD  CONSTRAINT [FK_Subscription_SubscriptionType] FOREIGN KEY([SubscriptionTypeId])
REFERENCES [dbo].[SubscriptionType] ([SubscriptionTypeId])
GO
ALTER TABLE [dbo].[Subscription] CHECK CONSTRAINT [FK_Subscription_SubscriptionType]
GO