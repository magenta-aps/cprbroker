/****** Object:  Table [dbo].[PersonBirthdate] ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PersonBirthdate]') AND type in (N'U'))
DROP TABLE [dbo].[PersonBirthdate]
GO

/****** Object:  Table [dbo].[PersonBirthdate] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PersonBirthdate]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[PersonBirthdate](
	[PersonUuid] [uniqueidentifier] NOT NULL,
	[Birthdate] [datetime] NOT NULL,
 CONSTRAINT [PK_PersonBirthdate] PRIMARY KEY CLUSTERED 
(
	[PersonUuid] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO