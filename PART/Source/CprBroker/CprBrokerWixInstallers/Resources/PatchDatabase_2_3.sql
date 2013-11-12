-- Create table DataProviderCall to keep track of calls made to dataproviders
/****** Object:  Table [dbo].[DataProviderCall]    Script Date: 11/12/2013 14:07:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DataProviderCall]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DataProviderCall](
	[DataProviderCallId] [uniqueidentifier] NOT NULL,
	[ActivityI] [uniqueidentifier] NOT NULL,
	[CallTime] [datetime] NOT NULL,
	[DataProviderType] [varchar](250) NOT NULL,
	[Cost] [decimal](18, 4) NOT NULL,
	[Operation] [varchar](250) NOT NULL,
	[Input] [varchar](250) NOT NULL,
	[Success] [bit] NOT NULL,
 CONSTRAINT [PK_DataProviderCall] PRIMARY KEY NONCLUSTERED 
(
	[DataProviderCallId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING ON
GO
/****** Object:  Default [DF__DataProviderCall_DataProviderCallId]    Script Date: 11/12/2013 14:07:02 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF__DataProviderCall_DataProviderCallId]') AND parent_object_id = OBJECT_ID(N'[dbo].[DataProviderCall]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__DataProviderCall_DataProviderCallId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[DataProviderCall] ADD  CONSTRAINT [DF__DataProviderCall_DataProviderCallId]  DEFAULT (newid()) FOR [DataProviderCallId]
END


End
GO
/****** Object:  Default [DF_DataProviderCall_CallTime]    Script Date: 11/12/2013 14:07:02 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_DataProviderCall_CallTime]') AND parent_object_id = OBJECT_ID(N'[dbo].[DataProviderCall]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_DataProviderCall_CallTime]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[DataProviderCall] ADD  CONSTRAINT [DF_DataProviderCall_CallTime]  DEFAULT (getdate()) FOR [CallTime]
END


End
GO


-- Add the field ActivityID to LogEntry in order to link it to rows in DataProviderCall
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('LogEntry') and name = 'ActivityId')
Begin
	alter table LogEntry add ActivityId uniqueidentifier
End