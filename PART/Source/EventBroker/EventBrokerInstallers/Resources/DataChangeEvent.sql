/****** Object:  Table [dbo].[DataChangeEvent] ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DataChangeEvent]') AND type in (N'U'))
DROP TABLE [dbo].[DataChangeEvent]
GO

/****** Object:  Table [dbo].[DataChangeEvent] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DataChangeEvent]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DataChangeEvent](
	[DataChangeEventId] [uniqueidentifier] NOT NULL,
	[PersonUuid] [uniqueidentifier] NOT NULL,
	[PersonRegistrationId] [uniqueidentifier] NOT NULL,
	[DueDate] [datetime] NOT NULL,
	[ReceivedDate] [datetime] NOT NULL,
    [ReceivedOrder] int NOT NULL IDENTITY (1, 1)
 CONSTRAINT [PK_DataChangeEvent] PRIMARY KEY CLUSTERED 
(
	[DataChangeEventId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
