/****** Object:  ForeignKey [FK_SubscriptionPerson_Subscription]    Script Date: 02/13/2011 17:59:04 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SubscriptionPerson_Subscription]') AND parent_object_id = OBJECT_ID(N'[dbo].[SubscriptionPerson]'))
ALTER TABLE [dbo].[SubscriptionPerson] DROP CONSTRAINT [FK_SubscriptionPerson_Subscription]
GO
/****** Object:  Default [DF_SubscriptionPerson_SubscriptionPersonId]    Script Date: 02/13/2011 17:59:04 ******/
IF  EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_SubscriptionPerson_SubscriptionPersonId]') AND parent_object_id = OBJECT_ID(N'[dbo].[SubscriptionPerson]'))
Begin
ALTER TABLE [dbo].[SubscriptionPerson] DROP CONSTRAINT [DF_SubscriptionPerson_SubscriptionPersonId]

End
GO
/****** Object:  Table [dbo].[SubscriptionPerson]    Script Date: 02/13/2011 17:59:04 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SubscriptionPerson]') AND type in (N'U'))
DROP TABLE [dbo].[SubscriptionPerson]
GO
/****** Object:  Table [dbo].[SubscriptionPerson]    Script Date: 02/13/2011 17:59:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SubscriptionPerson]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[SubscriptionPerson](
	[SubscriptionPersonId] [uniqueidentifier] NOT NULL,
	[SubscriptionId] [uniqueidentifier] NULL,
	[PersonUuid] [uniqueidentifier] NULL,
	[Created] [Datetime] NOT NULL,
	[Removed] [Datetime] NULL,
 CONSTRAINT [PK_SubscriptionPerson] PRIMARY KEY CLUSTERED 
(
	[SubscriptionPersonId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF),
 CONSTRAINT [IX_SubscriptionPerson] UNIQUE NONCLUSTERED 
(
	[SubscriptionId] ASC,
	[PersonUuid] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
/****** Object:  Default [DF_SubscriptionPerson_SubscriptionPersonId]    Script Date: 02/13/2011 17:59:04 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_SubscriptionPerson_SubscriptionPersonId]') AND parent_object_id = OBJECT_ID(N'[dbo].[SubscriptionPerson]'))
Begin
ALTER TABLE [dbo].[SubscriptionPerson] ADD  CONSTRAINT [DF_SubscriptionPerson_SubscriptionPersonId]  DEFAULT (newid()) FOR [SubscriptionPersonId]

End
GO
/****** Object:  ForeignKey [FK_SubscriptionPerson_Subscription]    Script Date: 02/13/2011 17:59:04 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SubscriptionPerson_Subscription]') AND parent_object_id = OBJECT_ID(N'[dbo].[SubscriptionPerson]'))
ALTER TABLE [dbo].[SubscriptionPerson]  WITH CHECK ADD  CONSTRAINT [FK_SubscriptionPerson_Subscription] FOREIGN KEY([SubscriptionId])
REFERENCES [dbo].[Subscription] ([SubscriptionId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SubscriptionPerson] CHECK CONSTRAINT [FK_SubscriptionPerson_Subscription]
GO
