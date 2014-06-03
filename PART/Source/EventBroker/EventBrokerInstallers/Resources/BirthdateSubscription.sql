/****** Object:  ForeignKey [FK_BirthdateSubscription_Subscription]    Script Date: 02/13/2011 17:59:04 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BirthdateSubscription_Subscription]') AND parent_object_id = OBJECT_ID(N'[dbo].[BirthdateSubscription]'))
ALTER TABLE [dbo].[BirthdateSubscription] DROP CONSTRAINT [FK_BirthdateSubscription_Subscription]
GO
/****** Object:  Default [DF_BirthdateSubscription_SubscriptionId]    Script Date: 02/13/2011 17:59:04 ******/
IF  EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_BirthdateSubscription_SubscriptionId]') AND parent_object_id = OBJECT_ID(N'[dbo].[BirthdateSubscription]'))
Begin
ALTER TABLE [dbo].[BirthdateSubscription] DROP CONSTRAINT [DF_BirthdateSubscription_SubscriptionId]

End
GO
/****** Object:  Default [DF_BirthdateSubscription_OffsetDays]    Script Date: 02/13/2011 17:59:04 ******/
IF  EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_BirthdateSubscription_OffsetDays]') AND parent_object_id = OBJECT_ID(N'[dbo].[BirthdateSubscription]'))
Begin
ALTER TABLE [dbo].[BirthdateSubscription] DROP CONSTRAINT [DF_BirthdateSubscription_OffsetDays]

End
GO
/****** Object:  Table [dbo].[BirthdateSubscription]    Script Date: 02/13/2011 17:59:04 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BirthdateSubscription]') AND type in (N'U'))
DROP TABLE [dbo].[BirthdateSubscription]
GO
/****** Object:  Table [dbo].[BirthdateSubscription]    Script Date: 02/13/2011 17:59:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BirthdateSubscription]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[BirthdateSubscription](
	[SubscriptionId] [uniqueidentifier] NOT NULL,
	[AgeYears] [int] NULL,
	[PriorDays] [int] NOT NULL,
 CONSTRAINT [PK_BirthdateSubscription] PRIMARY KEY CLUSTERED 
(
	[SubscriptionId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
/****** Object:  Default [DF_BirthdateSubscription_SubscriptionId]    Script Date: 02/13/2011 17:59:04 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_BirthdateSubscription_SubscriptionId]') AND parent_object_id = OBJECT_ID(N'[dbo].[BirthdateSubscription]'))
Begin
ALTER TABLE [dbo].[BirthdateSubscription] ADD  CONSTRAINT [DF_BirthdateSubscription_SubscriptionId]  DEFAULT (newid()) FOR [SubscriptionId]

End
GO
/****** Object:  Default [DF_BirthdateSubscription_OffsetDays]    Script Date: 02/13/2011 17:59:04 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_BirthdateSubscription_OffsetDays]') AND parent_object_id = OBJECT_ID(N'[dbo].[BirthdateSubscription]'))
Begin
ALTER TABLE [dbo].[BirthdateSubscription] ADD  CONSTRAINT [DF_BirthdateSubscription_OffsetDays]  DEFAULT ((0)) FOR [PriorDays]

End
GO
/****** Object:  ForeignKey [FK_BirthdateSubscription_Subscription]    Script Date: 02/13/2011 17:59:04 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BirthdateSubscription_Subscription]') AND parent_object_id = OBJECT_ID(N'[dbo].[BirthdateSubscription]'))
ALTER TABLE [dbo].[BirthdateSubscription]  WITH CHECK ADD  CONSTRAINT [FK_BirthdateSubscription_Subscription] FOREIGN KEY([SubscriptionId])
REFERENCES [dbo].[Subscription] ([SubscriptionId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BirthdateSubscription] CHECK CONSTRAINT [FK_BirthdateSubscription_Subscription]
GO