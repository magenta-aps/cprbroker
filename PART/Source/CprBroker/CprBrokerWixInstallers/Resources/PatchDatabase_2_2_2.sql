ALTER TABLE DataProviderCall ALTER COLUMN Success bit NULL

/****** Object:  Table [dbo].[BudgetInterval]    Script Date: 11/28/2013 16:33:20 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[BudgetInterval](
	[IntervalMilliseconds] [bigint] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[CallThreshold] [int] NULL,
	[CostThreshold] [decimal](18, 4) NULL,
	[LastChecked] [datetime] NULL,
 CONSTRAINT [PK_BudgetInterval] PRIMARY KEY CLUSTERED 
(
	[IntervalMillisecods] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING ON
GO


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


-- =============================================
-- Procedure:   InitializePersonSearchCache
-- Author:		Beemen Beshara
-- Create date: 24-Jan-2014
-- Description:	Initializes the cashed version of persons' searchable fields
-- =============================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'InitializePersonSearchCache')
	DROP PROCEDURE dbo.InitializePersonSearchCache
GO

CREATE PROCEDURE [dbo].[InitializePersonSearchCache]
	@UUID UNIQUEIDENTIFIER, 
	@PersonRegistrationId UNIQUEIDENTIFIER, 
	@RegistrationDate DATETIME, 
	@Contents XML
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
	SET NOCOUNT ON;
    
    -- Determine if this is the latest registration
	IF NOT EXISTS (
		SELECT PersonRegistrationId 
		FROM PersonRegistration 
		WHERE 
			UUID = @UUID 
			AND RegistrationDate > @RegistrationDate
		)
	BEGIN
		-- Ensure record exists
		IF NOT EXISTS (SELECT UUID FROM PersonSearchCache WHERE UUID = @UUID)
		BEGIN
			DECLARE @C INT		
			INSERT INTO PersonSearchCache(UUID) VALUES (@UUID)
		END
		
		-- Fill the data
		UPDATE  PersonSearchCache
		SET 
			PersonRegistrationId = @PersonRegistrationId,
			UserInterfaceKeyText = Convert(VARCHAR(MAX), tmp_UserInterfaceKeyText.Value.query('text()')),
			BirthDate = Convert(VARCHAR(MAX), tmp_BirthDate.Value.query('text()')),
			NickName = Convert(VARCHAR(MAX), tmp_NickName.Value.query('text()')),
			Note = Convert(VARCHAR(MAX), tmp_Note.Value.query('text()')),
			AddressingName = Convert(VARCHAR(MAX), tmp_AddressingName.Value.query('text()')),
			PersonGivenName = CONVERT(VARCHAR(MAX), tmp_PersonGivenName.Value.query('text()')), 
			PersonMiddleName = CONVERT(VARCHAR(MAX), tmp_PersonMiddleName.Value.query('text()')), 
			PersonSurnameName = CONVERT(VARCHAR(MAX), tmp_PersonSurnameName.Value.query('text()')), 
			PersonGenderCode = CONVERT(VARCHAR(MAX), tmp_PersonGenderCode.Value.query('text()')), 
			LivscyklusKode = CONVERT(VARCHAR(MAX), tmp_LivscyklusKode.Value.query('text()'))

		FROM 
			PersonSearchCache
			
			OUTER APPLY @Contents.nodes ('declare namespace ns0="urn:oio:sagdok:person:1.0.0";
			declare namespace ns1="http://rep.oio.dk/cpr.dk/xml/schemas/core/2005/03/18/";
			(/ns0:Registrering/ns0:AttributListe/ns0:RegisterOplysning/ns0:CprBorger/ns1:PersonCivilRegistrationIdentifier)[last()]'
			) AS tmp_UserInterfaceKeyText (Value)
			
			OUTER APPLY @Contents.nodes ('declare namespace ns0="urn:oio:sagdok:person:1.0.0";
			declare namespace ns1="http://rep.oio.dk/ebxml/xml/schemas/dkcc/2005/03/15/";
			(/ns0:Registrering/ns0:AttributListe/ns0:Egenskab/ns1:BirthDate)[last()]'
			) AS tmp_BirthDate (Value)
			
			OUTER APPLY @Contents.nodes ('declare namespace ns0="urn:oio:sagdok:person:1.0.0";
			declare namespace ns1="urn:oio:sagdok:2.0.0";
			(/ns0:Registrering/ns0:AttributListe/ns0:Egenskab/ns0:NavnStruktur/ns1:KaldenavnTekst)[last()]'
			) AS tmp_NickName (Value)
			
			OUTER APPLY @Contents.nodes ('declare namespace ns0="urn:oio:sagdok:person:1.0.0";
			declare namespace ns1="urn:oio:sagdok:2.0.0";
			(/ns0:Registrering/ns0:AttributListe/ns0:Egenskab/ns0:NavnStruktur/ns1:NoteTekst)[last()]'
			) AS tmp_Note (Value)
			
			OUTER APPLY @Contents.nodes ('declare namespace ns0="urn:oio:sagdok:person:1.0.0";
			declare namespace ns1="http://rep.oio.dk/itst.dk/xml/schemas/2005/02/22/";
			(/ns0:Registrering/ns0:AttributListe/ns0:Egenskab/ns0:NavnStruktur/ns1:PersonNameForAddressingName)[last()]'
			) AS tmp_AddressingName (Value)
			
			OUTER APPLY @Contents.nodes ('declare namespace ns0="urn:oio:sagdok:person:1.0.0";
			declare namespace ns1="http://rep.oio.dk/itst.dk/xml/schemas/2006/01/17/";
			declare namespace ns2="http://rep.oio.dk/ebxml/xml/schemas/dkcc/2003/02/13/";
			(/ns0:Registrering/ns0:AttributListe/ns0:Egenskab/ns0:NavnStruktur/ns1:PersonNameStructure/ns2:PersonGivenName)[last()]'
			) AS tmp_PersonGivenName (Value)

			OUTER APPLY @Contents.nodes ('declare namespace ns0="urn:oio:sagdok:person:1.0.0";
			declare namespace ns1="http://rep.oio.dk/itst.dk/xml/schemas/2006/01/17/";
			declare namespace ns2="http://rep.oio.dk/ebxml/xml/schemas/dkcc/2003/02/13/";
			(/ns0:Registrering/ns0:AttributListe/ns0:Egenskab/ns0:NavnStruktur/ns1:PersonNameStructure/ns2:PersonMiddleName)[last()]'
			) AS tmp_PersonMiddleName (Value)

			OUTER APPLY @Contents.nodes ('declare namespace ns0="urn:oio:sagdok:person:1.0.0";
			declare namespace ns1="http://rep.oio.dk/itst.dk/xml/schemas/2006/01/17/";
			declare namespace ns2="http://rep.oio.dk/ebxml/xml/schemas/dkcc/2003/02/13/";
			(/ns0:Registrering/ns0:AttributListe/ns0:Egenskab/ns0:NavnStruktur/ns1:PersonNameStructure/ns2:PersonSurnameName)[last()]'
			) AS tmp_PersonSurnameName (Value)

			OUTER APPLY @Contents.nodes ('declare namespace ns0="urn:oio:sagdok:person:1.0.0";
			declare namespace ns1="http://rep.oio.dk/ebxml/xml/schemas/dkcc/2006/01/23/";
			(/ns0:Registrering/ns0:AttributListe/ns0:Egenskab/ns1:PersonGenderCode)[last()]'
			) AS tmp_PersonGenderCode (Value)

			OUTER APPLY @Contents.nodes ('declare namespace ns0="urn:oio:sagdok:person:1.0.0";
			declare namespace ns1="urn:oio:sagdok:2.0.0";
			(/ns0:Registrering/ns1:LivscyklusKode)[last()]'
			) AS tmp_LivscyklusKode (Value)

			

		WHERE
			UUID = @UUID
	END
		
END
GO

-- =============================================
-- Author:		Beemen Beshara
-- Description:	Trigger for changes in PersonRegistration, 
--  refreshes the cached serach table by calling InitializePersonSearchCache for 
--  each record being inserted or updated
-- =============================================

IF EXISTS (SELECT * FROM sys.triggers where name='PersonRegistration_PopulateSearchCache')
BEGIN
	DROP TRIGGER dbo.PersonRegistration_PopulateSearchCache
END
GO

CREATE TRIGGER dbo.PersonRegistration_PopulateSearchCache
   ON  dbo.PersonRegistration
   AFTER INSERT,UPDATE
AS 
BEGIN
	--SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @UUID UNIQUEIDENTIFIER, @PersonRegistrationId UNIQUEIDENTIFIER, @RegistrationDate DATETIME, @Contents XML
	
	DECLARE CUR Cursor FOR
	SELECT UUID, PersonRegistrationId, RegistrationDate , Contents
	FROM    INSERTED
	
	OPEN CUR
	FETCH NEXT FROM CUR INTO @UUID, @PersonRegistrationId, @RegistrationDate, @Contents
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
		EXEC dbo.InitializePersonSearchCache @UUID, @PersonRegistrationId, @RegistrationDate, @Contents
		FETCH NEXT FROM CUR INTO @UUID, @PersonRegistrationId, @RegistrationDate, @Contents
	END
	
	CLOSE CUR
	DEALLOCATE CUR

END
GO



