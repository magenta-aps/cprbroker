/****** Object:  Table [dbo].[LifecycleStatus]    Script Date: 11/21/2013 10:16:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LifecycleStatus]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[LifecycleStatus](
	[LifecycleStatusId] [int] NOT NULL,
	[LifecycleStatusName] [varchar](50) NULL,
 CONSTRAINT [PK_LifecycleStatus] PRIMARY KEY CLUSTERED 
(
	[LifecycleStatusId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
