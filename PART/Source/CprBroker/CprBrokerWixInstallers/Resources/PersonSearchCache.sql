-- ========================================================
-- Table  : PersonSearchCache
-- ========================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE Name = 'PersonSearchCache')
BEGIN
	CREATE TABLE [dbo].[PersonSearchCache](
		[PersonRegistrationId] [uniqueidentifier] NULL,
		[UUID] [uniqueidentifier] NOT NULL,
		[UserInterfaceKeyText] [varchar](max) NULL,
		[Birthdate] [varchar](max) NULL,
		[NickName] [varchar](max) NULL,
		[Note] [varchar](max) NULL,
		[AddressingName] [varchar](max) NULL,
		[PersonGivenName] [varchar](max) NULL,
		[PersonMiddleName] [varchar](max) NULL,
		[PersonSurnameName] [varchar](max) NULL,
		[PersonGenderCode] [varchar](max) NULL,
		[LivscyklusKode] [varchar](max) NULL,
	 CONSTRAINT [PK_PersonSearchCache] PRIMARY KEY CLUSTERED 
	(
		[UUID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END

GO
