-- ========================================================
-- Table  : PersonSearchCache
-- ========================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE Name = 'PersonSearchCache')
BEGIN
	CREATE TABLE [dbo].[PersonSearchCache](
		-- Root fields
        [PersonRegistrationId] [uniqueidentifier] NULL,
		[UUID] [uniqueidentifier] NOT NULL,		
        LivscyklusKode VARCHAR(MAX),
            
        -- Egenskab fields
        AddressingName VARCHAR(MAX), NickName VARCHAR(MAX), Note VARCHAR(MAX), 
        PersonGivenName VARCHAR(MAX), PersonMiddleName VARCHAR(MAX), PersonSurnameName VARCHAR(MAX), 
        PersonGenderCode VARCHAR(MAX), Birthdate DATETIME, FoedestedNavn VARCHAR(MAX), FoedselsregistreringMyndighedNavn VARCHAR(MAX),
            
        -- CprBorger
        UserInterfaceKeyText VARCHAR(MAX), PersonCivilRegistrationIdentifier VARCHAR(MAX),
        PersonNummerGyldighedStatusIndikator bit, PersonNationalityCode VARCHAR(MAX),
        NavneAdresseBeskyttelseIndikator bit, TelefonNummerBeskyttelseIndikator bit, ForskerBeskyttelseIndikator bit,
            
        -- CprBorger - after address
        AdresseNoteTekst VARCHAR(MAX), FolkekirkeMedlemIndikator bit,

        -- FolkeregisterAdresse
        NoteTekst_DanskAdresse VARCHAR(MAX), UkendtAdresseIndikator VARCHAR(MAX),
        SpecielVejkodeIndikator VARCHAR(MAX), PostDistriktTekst VARCHAR(MAX),

        -- AddressAccess
        MunicipalityCode VARCHAR(MAX), StreetCode VARCHAR(MAX), StreetBuildingIdentifier VARCHAR(MAX),

        -- AddressPostal
        MailDeliverySublocationIdentifier VARCHAR(MAX), StreetName VARCHAR(MAX), StreetNameForAddressingName VARCHAR(MAX),
        StreetBuildingIdentifier_Postal VARCHAR(MAX), FloorIdentifier VARCHAR(MAX), SuiteIdentifier VARCHAR(MAX),
        DistrictSubdivisionIdentifier VARCHAR(MAX), PostOfficeBoxIdentifier VARCHAR(MAX),
        PostCodeIdentifier VARCHAR(MAX), DistrictName VARCHAR(MAX),
        CountryIdentificationCode VARCHAR(MAX)

	 CONSTRAINT [PK_PersonSearchCache] PRIMARY KEY CLUSTERED 
	(
		[UUID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END

GO
