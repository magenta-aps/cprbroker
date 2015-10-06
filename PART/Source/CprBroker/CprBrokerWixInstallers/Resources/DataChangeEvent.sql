/****** Object:  Table [dbo].[DataChangeEvent]    Script Date: 11/21/2013 10:16:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DataChangeEvent]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DataChangeEvent](
	[DataChangeEventId] [uniqueidentifier] NOT NULL
		DEFAULT NEWID()
		CONSTRAINT [PK_DataChangeEvent] PRIMARY KEY CLUSTERED (	[DataChangeEventId] ASC),
	[PersonUuid] [uniqueidentifier] NOT NULL,
	[PersonRegistrationId] [uniqueidentifier] NOT NULL,
	[ReceivedDate] [datetime] NOT NULL,
) ON [PRIMARY]
END

GO