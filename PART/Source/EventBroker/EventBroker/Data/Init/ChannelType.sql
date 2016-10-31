/****** Object:  Table [dbo].[ChannelType]    Script Date: 02/13/2011 17:59:04 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ChannelType]') AND type in (N'U'))
DROP TABLE [dbo].[ChannelType]
GO
/****** Object:  Table [dbo].[ChannelType]    Script Date: 02/13/2011 17:59:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ChannelType]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ChannelType](
	[ChannelTypeId] [int] NOT NULL,
	[Name] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_ChannelType] PRIMARY KEY CLUSTERED 
(
	[ChannelTypeId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO