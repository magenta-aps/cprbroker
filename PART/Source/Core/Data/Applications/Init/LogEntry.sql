/****** Object:  Table [dbo].[LogEntry]    Script Date: 11/21/2013 10:16:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LogEntry]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[LogEntry](
	[LogEntryId] [uniqueidentifier] NOT NULL,
	[LogTypeId] [int] NOT NULL,
	[ApplicationId] [uniqueidentifier] NULL,
	[UserToken] [varchar](250) NULL,
	[UserId] [varchar](250) NULL,
	[MethodName] [varchar](250) NULL,
	[Text] [nvarchar](max) NULL,
	[DataObjectType] [varchar](250) NULL,
	[DataObjectXml] [ntext] NULL,
	[LogDate] [datetime] NOT NULL,
	[ActivityId] [uniqueidentifier] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[LogEntry]') AND name = N'IX_LogEntry_LogDate')
CREATE CLUSTERED INDEX [IX_LogEntry_LogDate] ON [dbo].[LogEntry] 
(
	[LogDate] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[LogEntry]') AND name = N'PK_LogEntry')
CREATE UNIQUE NONCLUSTERED INDEX [PK_LogEntry] ON [dbo].[LogEntry] 
(
	[LogEntryId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

/****** Object:  ForeignKey [FK_LogEntry_Application]    Script Date: 11/21/2013 10:16:51 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LogEntry_Application]') AND parent_object_id = OBJECT_ID(N'[dbo].[LogEntry]'))
ALTER TABLE [dbo].[LogEntry]  WITH NOCHECK ADD  CONSTRAINT [FK_LogEntry_Application] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[Application] ([ApplicationId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LogEntry_Application]') AND parent_object_id = OBJECT_ID(N'[dbo].[LogEntry]'))
ALTER TABLE [dbo].[LogEntry] NOCHECK CONSTRAINT [FK_LogEntry_Application]
GO
/****** Object:  ForeignKey [FK_LogEntry_LogType]    Script Date: 11/21/2013 10:16:51 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LogEntry_LogType]') AND parent_object_id = OBJECT_ID(N'[dbo].[LogEntry]'))
ALTER TABLE [dbo].[LogEntry]  WITH CHECK ADD  CONSTRAINT [FK_LogEntry_LogType] FOREIGN KEY([LogTypeId])
REFERENCES [dbo].[LogType] ([LogTypeId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LogEntry_LogType]') AND parent_object_id = OBJECT_ID(N'[dbo].[LogEntry]'))
ALTER TABLE [dbo].[LogEntry] CHECK CONSTRAINT [FK_LogEntry_LogType]
GO
