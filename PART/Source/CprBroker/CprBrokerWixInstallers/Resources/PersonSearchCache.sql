-- ========================================================
-- Table  : PersonSearchCache
-- ========================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE Name = 'PersonSearchCache')
BEGIN
    CREATE TABLE [dbo].[PersonSearchCache](
	    -- Root fields
        [PersonRegistrationId] [uniqueidentifier] NULL,
	    [UUID] [uniqueidentifier] NOT NULL,		
        LivscyklusKode NVARCHAR(MAX),
            
        -- Egenskab fields
        AddressingName NVARCHAR(MAX), NickName NVARCHAR(MAX), Note NVARCHAR(MAX), 
        PersonGivenName NVARCHAR(MAX), PersonMiddleName NVARCHAR(MAX), PersonSurnameName NVARCHAR(MAX), 
        PersonGenderCode NVARCHAR(MAX), Birthdate DATETIME, FoedestedNavn NVARCHAR(MAX), FoedselsregistreringMyndighedNavn NVARCHAR(MAX),

        -- CprBorger
        UserInterfaceKeyText NVARCHAR(MAX), PersonCivilRegistrationIdentifier NVARCHAR(MAX),
        PersonNummerGyldighedStatusIndikator bit, PersonNationalityCode NVARCHAR(MAX),
        NavneAdresseBeskyttelseIndikator bit, TelefonNummerBeskyttelseIndikator bit, ForskerBeskyttelseIndikator bit,
            
        -- CprBorger - after address
        AdresseNoteTekst NVARCHAR(MAX), FolkekirkeMedlemIndikator bit,

        -- FolkeregisterAdresse
        AddressType char, 
        NoteTekst_DanskAdresse NVARCHAR(MAX), UkendtAdresseIndikator bit,
        SpecielVejkodeIndikator bit, PostDistriktTekst NVARCHAR(MAX),

        -- AddressAccess
        -- TODO Municilaiy , StreetCode as int??
        MunicipalityCode NVARCHAR(MAX), StreetCode NVARCHAR(MAX), StreetBuildingIdentifier NVARCHAR(MAX),

        -- AddressPostal
        MailDeliverySublocationIdentifier NVARCHAR(MAX), StreetName NVARCHAR(MAX), StreetNameForAddressingName NVARCHAR(MAX),
        StreetBuildingIdentifier_Postal NVARCHAR(MAX), FloorIdentifier NVARCHAR(MAX), SuiteIdentifier NVARCHAR(MAX),
        DistrictSubdivisionIdentifier NVARCHAR(MAX), PostOfficeBoxIdentifier NVARCHAR(MAX),
        PostCodeIdentifier NVARCHAR(MAX), DistrictName NVARCHAR(MAX),
        CountryIdentificationCode NVARCHAR(MAX),

        -- Greenlandic address
        GreenlandBuildingIdentifier NVARCHAR(MAX),

        -- World address
        LocationDescriptionText NVARCHAR(MAX),
        PostalAddressFirstLineText NVARCHAR(MAX), PostalAddressSecondLineText NVARCHAR(MAX), PostalAddressThirdLineText NVARCHAR(MAX), 
        PostalAddressFourthLineText NVARCHAR(MAX), PostalAddressFifthLineText NVARCHAR(MAX)

	    CONSTRAINT [PK_PersonSearchCache] PRIMARY KEY CLUSTERED (	[UUID] ASC)
        WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
    ) ON [PRIMARY]
END

GO