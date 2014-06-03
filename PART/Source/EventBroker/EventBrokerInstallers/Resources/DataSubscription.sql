/****** Object:  ForeignKey [FK_DataSubscription_Subscription]    Script Date: 02/13/2011 17:59:04 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DataSubscription_Subscription]') AND parent_object_id = OBJECT_ID(N'[dbo].[DataSubscription]'))
ALTER TABLE [dbo].[DataSubscription] DROP CONSTRAINT [FK_DataSubscription_Subscription]
GO
/****** Object:  Table [dbo].[DataSubscription]    Script Date: 02/13/2011 17:59:04 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DataSubscription]') AND type in (N'U'))
DROP TABLE [dbo].[DataSubscription]
GO
/****** Object:  Table [dbo].[DataSubscription]    Script Date: 02/13/2011 17:59:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DataSubscription]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DataSubscription](
	[SubscriptionId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_DataSubscription] PRIMARY KEY CLUSTERED 
(
	[SubscriptionId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
/****** Object:  ForeignKey [FK_DataSubscription_Subscription]    Script Date: 02/13/2011 17:59:04 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DataSubscription_Subscription]') AND parent_object_id = OBJECT_ID(N'[dbo].[DataSubscription]'))
ALTER TABLE [dbo].[DataSubscription]  WITH CHECK ADD  CONSTRAINT [FK_DataSubscription_Subscription] FOREIGN KEY([SubscriptionId])
REFERENCES [dbo].[Subscription] ([SubscriptionId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DataSubscription] CHECK CONSTRAINT [FK_DataSubscription_Subscription]
GO