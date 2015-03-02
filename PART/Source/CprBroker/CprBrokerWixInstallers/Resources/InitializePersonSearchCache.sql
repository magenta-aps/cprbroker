/* 
  ===========================================================================
    Procedure:   InitializePersonSearchCache
    Author:		 Beemen Beshara
    Create date: 24-Jan-2014
    Description: Initializes the cashed version of persons' searchable fields
 ============================================================================
*/

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
    -- If not, quit
    IF EXISTS (
        SELECT PersonRegistrationId 
        FROM PersonRegistration 
        WHERE 
            UUID = @UUID 
            AND RegistrationDate > @RegistrationDate
        )
    BEGIN
        RETURN
    END

    -------------------
    -- Root variables
    -------------------
    DECLARE @ContentsTable Table (X XML)
    DECLARE @LivscyklusKode NVARCHAR(MAX);
    DECLARE @EgenskabNode XML, @RegisterOplysningNode XML;
    
    INSERT INTO @ContentsTable SELECT @Contents;

    -- Select the nodes that are currently in effect
    WITH XMLNAMESPACES(
        'urn:oio:sagdok:person:1.0.0' as ns0,
        'urn:oio:sagdok:2.0.0' as ns1 )
    SELECT 
        @EgenskabNode           = X.query('(/ns0:Registrering/ns0:AttributListe/ns0:Egenskab[ns1:Virkning/ns1:TilTidspunkt/ns1:GraenseIndikator])'),
        @RegisterOplysningNode  = X.query('(/ns0:Registrering/ns0:AttributListe/ns0:RegisterOplysning[ns1:Virkning/ns1:TilTidspunkt/ns1:GraenseIndikator])'),
        @LivscyklusKode         = X.value('(/ns0:Registrering/ns1:LivscyklusKode)[last()]','NVARCHAR(max)')
    FROM 
        @ContentsTable

    --------------
    -- Egenskab --
    --------------
    DECLARE @AddressingName NVARCHAR(MAX), @NickName NVARCHAR(MAX), @Note NVARCHAR(MAX), 
        @PersonGivenName NVARCHAR(MAX), @PersonMiddleName NVARCHAR(MAX), @PersonSurnameName NVARCHAR(MAX), 
        @PersonGenderCode NVARCHAR(MAX), @Birthdate Datetime, @FoedestedNavn NVARCHAR(MAX), @FoedselsregistreringMyndighedNavn NVARCHAR(MAX)

    IF LEN (CAST (@EgenskabNode AS NVARCHAR(MAX))) > 0
    BEGIN	
        DECLARE @EgenskabTable TABLE (X XML);
        INSERT INTO @EgenskabTable SELECT @EgenskabNode;
    
        WITH XMLNAMESPACES (
            'urn:oio:sagdok:person:1.0.0' as ns0,
            'http://rep.oio.dk/itst.dk/xml/schemas/2005/02/22/' as ns1,
            'urn:oio:sagdok:2.0.0' as ns2,
            'http://rep.oio.dk/itst.dk/xml/schemas/2006/01/17/' as ns3,
            'http://rep.oio.dk/ebxml/xml/schemas/dkcc/2003/02/13/' as ns4,
            'http://rep.oio.dk/ebxml/xml/schemas/dkcc/2006/01/23/' as ns5,
            'http://rep.oio.dk/ebxml/xml/schemas/dkcc/2005/03/15/' as ns6) 
        SELECT 
            @AddressingName                     = X.value('(/ns0:Egenskab/ns0:NavnStruktur/ns1:PersonNameForAddressingName)[last()]'               , 'NVARCHAR(max)'),
            @NickName                           = X.value('(/ns0:Egenskab/ns0:NavnStruktur/ns2:KaldenavnTekst)[last()]'                            , 'NVARCHAR(max)'),
            @Note                               = X.value('(/ns0:Egenskab/ns0:NavnStruktur/ns2:NoteTekst)[last()]'                                 , 'NVARCHAR(max)'),		
            @PersonGivenName                    = X.value('(/ns0:Egenskab/ns0:NavnStruktur/ns3:PersonNameStructure/ns4:PersonGivenName)[last()]'   , 'NVARCHAR(max)'),
            @PersonMiddleName                   = X.value('(/ns0:Egenskab/ns0:NavnStruktur/ns3:PersonNameStructure/ns4:PersonMiddleName)[last()]'  , 'NVARCHAR(max)'),
            @PersonSurnameName                  = X.value('(/ns0:Egenskab/ns0:NavnStruktur/ns3:PersonNameStructure/ns4:PersonSurnameName)[last()]' , 'NVARCHAR(max)'),
            @PersonGenderCode                   = X.value('(/ns0:Egenskab/ns5:PersonGenderCode)[last()]'                                           , 'NVARCHAR(max)'),
            @Birthdate                          = X.value('(/ns0:Egenskab/ns6:BirthDate)[last()]'                                                  , 'datetime'),
            @FoedestedNavn                      = X.value('(/ns0:Egenskab/ns0:FoedestedNavn)[last()]'                                              , 'NVARCHAR(max)'),
            -- TODO: This field is not used !!
            @FoedselsregistreringMyndighedNavn  = X.value('(/ns0:Egenskab/ns0:FoedselsregistreringMyndighedNavn)[last()]'                          , 'NVARCHAR(max)')
        FROM 
            @EgenskabTable
    END

    -----------------------
    -- RegisterOplysning --
    -----------------------

    -- CprBorger fields
    DECLARE @UserInterfaceKeyText NVARCHAR(MAX), @PersonCivilRegistrationIdentifier NVARCHAR(max),
        @PersonNummerGyldighedStatusIndikator bit, @PersonNationalityCode NVARCHAR(MAX),
        @NavneAdresseBeskyttelseIndikator bit,@TelefonNummerBeskyttelseIndikator bit, @ForskerBeskyttelseIndikator bit;

    -- CprBorger fields - after address
    DECLARE @AdresseNoteTekst NVARCHAR(MAX), @FolkekirkeMedlemIndikator bit;
    
    -- FolkeregisterAdresse fields
    DECLARE @AddressType char, 
        @NoteTekst_DanskAdresse NVARCHAR(MAX), @UkendtAdresseIndikator bit, 
        @SpecielVejkodeIndikator bit, @PostDistriktTekst NVARCHAR(MAX);
    
    -- AddressAccess fields
    DECLARE @MunicipalityCode int, @StreetCode int, @StreetBuildingIdentifier NVARCHAR(MAX);
    
    -- AddressPostal fields
    DECLARE @MailDeliverySublocationIdentifier NVARCHAR(MAX),
        @StreetName NVARCHAR(MAX), @StreetNameForAddressingName NVARCHAR(MAX), 
        @StreetBuildingIdentifier_Postal NVARCHAR(MAX), @FloorIdentifier NVARCHAR(MAX), @SuiteIdentifier NVARCHAR(MAX), 
        @DistrictSubdivisionIdentifier NVARCHAR(MAX), @PostOfficeBoxIdentifier NVARCHAR(MAX), 
        @PostCodeIdentifier int, @DistrictName NVARCHAR(MAX), 
        @CountryIdentificationCode NVARCHAR(MAX);
        
    -- Greenlandic address
    DECLARE @GreenlandBuildingIdentifier NVARCHAR(MAX)

    -- World address
    DECLARE @LocationDescriptionText NVARCHAR(MAX),
        @PostalAddressFirstLineText NVARCHAR(MAX), @PostalAddressSecondLineText NVARCHAR(MAX), @PostalAddressThirdLineText NVARCHAR(MAX), 
        @PostalAddressFourthLineText NVARCHAR(MAX), @PostalAddressFifthLineText NVARCHAR(MAX)
    
    IF LEN (CAST (@RegisterOplysningNode AS NVARCHAR(MAX))) > 0
    BEGIN	
        DECLARE @RegisterOplysningTable TABLE (X XML);
        INSERT INTO @RegisterOplysningTable SELECT @RegisterOplysningNode;
    
        WITH XMLNAMESPACES (
            'urn:oio:sagdok:person:1.0.0' as ns0,
            'http://rep.oio.dk/cpr.dk/xml/schemas/core/2005/03/18/' as ns1,
            'http://rep.oio.dk/ebxml/xml/schemas/dkcc/2006/01/03/' as ns2)
        SELECT
            @UserInterfaceKeyText                  = X.value('(/ns0:RegisterOplysning/ns0:CprBorger/ns1:PersonCivilRegistrationIdentifier)[last()]'    , 'NVARCHAR(max)' ),
            @PersonCivilRegistrationIdentifier     = X.value('(/ns0:RegisterOplysning/ns0:CprBorger/ns1:PersonCivilRegistrationIdentifier)[last()]'    , 'NVARCHAR(max)' ),
            @PersonNummerGyldighedStatusIndikator  = X.value('(/ns0:RegisterOplysning/ns0:CprBorger/ns0:PersonNummerGyldighedStatusIndikator)[last()]' , 'bit'          ),
            @PersonNationalityCode                 = X.value('(/ns0:RegisterOplysning/ns0:CprBorger/ns2:PersonNationalityCode)[last()]'                , 'NVARCHAR(max)' ),
            @NavneAdresseBeskyttelseIndikator      = X.value('(/ns0:RegisterOplysning/ns0:CprBorger/ns0:NavneAdresseBeskyttelseIndikator)[last()]'     , 'bit'          ),
            @TelefonNummerBeskyttelseIndikator     = X.value('(/ns0:RegisterOplysning/ns0:CprBorger/ns0:TelefonNummerBeskyttelseIndikator)[last()]'    , 'bit'          ),
            @ForskerBeskyttelseIndikator           = X.value('(/ns0:RegisterOplysning/ns0:CprBorger/ns0:ForskerBeskyttelseIndikator)[last()]'          , 'bit'          )
        FROM @RegisterOplysningTable;

        -- CprBorger - after address
        WITH XMLNAMESPACES (
            'urn:oio:sagdok:person:1.0.0' as ns0,
            'http://rep.oio.dk/cpr.dk/xml/schemas/core/2005/03/18/' as ns1,
            'http://rep.oio.dk/ebxml/xml/schemas/dkcc/2006/01/03/' as ns2)
        SELECT
            @AdresseNoteTekst           = X.value('(/ns0:RegisterOplysning/ns0:CprBorger/ns0:AdresseNoteTekst)[last()]'          , 'NVARCHAR(max)'),
            @FolkekirkeMedlemIndikator  = X.value('(/ns0:RegisterOplysning/ns0:CprBorger/ns0:FolkekirkeMedlemIndikator)[last()]' , 'bit')
        FROM @RegisterOplysningTable;

        -- DanskAddress
        DECLARE @DanskAdresseTable TABLE(X XML);
        WITH XMLNAMESPACES ('urn:oio:sagdok:person:1.0.0' as ns0)
        INSERT INTO @DanskAdresseTable SELECT X.query('/ns0:RegisterOplysning/ns0:CprBorger/ns0:FolkeregisterAdresse/ns0:DanskAdresse') FROM @RegisterOplysningTable;
        
        IF (SELECT LEN(CAST(X AS NVARCHAR(MAX))) FROM @DanskAdresseTable) > 0
        BEGIN
            WITH XMLNAMESPACES (
                'urn:oio:sagdok:person:1.0.0' as ns0,
                'urn:oio:sagdok:2.0.0' AS ns1)
            SELECT 
                @AddressType             = 'D',
                @NoteTekst_DanskAdresse  = X.value('(/ns0:DanskAdresse/ns1:NoteTekst)[last()]'               , 'NVARCHAR(max)'),
                @UkendtAdresseIndikator  = X.value('(/ns0:DanskAdresse/ns0:UkendtAdresseIndikator)[last()]'  , 'bit'),
                @SpecielVejkodeIndikator = X.value('(/ns0:DanskAdresse/ns0:SpecielVejkodeIndikator)[last()]' , 'bit'),
                @PostDistriktTekst       = X.value('(/ns0:DanskAdresse/ns0:PostDistriktTekst)[last()]'       , 'NVARCHAR(max)')
            FROM @DanskAdresseTable;
        
            -- AddressAccess
            WITH XMLNAMESPACES (
                'urn:oio:sagdok:person:1.0.0' as ns0,
                'http://rep.oio.dk/xkom.dk/xml/schemas/2006/01/06/' AS ns1,
                'http://rep.oio.dk/xkom.dk/xml/schemas/2005/03/15/' as ns2,
                'http://rep.oio.dk/cpr.dk/xml/schemas/core/2005/03/18/' as ns3,            
                'http://rep.oio.dk/ebxml/xml/schemas/dkcc/2003/02/13/' as ns4 )
            SELECT
                @MunicipalityCode         =                         X.value('(/ns2:AddressAccess/ns3:MunicipalityCode)[last()]'         , 'int'),
                @StreetCode               =                         X.value('(/ns2:AddressAccess/ns3:StreetCode)[last()]'               , 'int'),
                @StreetBuildingIdentifier = dbo.TrimAddressString(  X.value('(/ns2:AddressAccess/ns4:StreetBuildingIdentifier)[last()]' , 'NVARCHAR(max)'))
            FROM 
                (SELECT X.query('/ns0:DanskAdresse/ns1:AddressComplete/ns2:AddressAccess') FROM @DanskAdresseTable) AS tmp_AddressPostalTable(X);

            -- AddressPostal
            WITH XMLNAMESPACES (
                'urn:oio:sagdok:person:1.0.0' as ns0,
                'http://rep.oio.dk/xkom.dk/xml/schemas/2006/01/06/' AS ns1,
                'http://rep.oio.dk/ebxml/xml/schemas/dkcc/2003/02/13/' AS ns2,
                'http://rep.oio.dk/ebxml/xml/schemas/dkcc/2005/03/15/' as ns3,
                'http://rep.oio.dk/cpr.dk/xml/schemas/core/2005/03/18/' as ns4,
                'http://rep.oio.dk/ebxml/xml/schemas/dkcc/2005/05/13/' as ns5
                )
            SELECT
                @MailDeliverySublocationIdentifier  =                       X.value('(/ns1:AddressPostal/ns2:MailDeliverySublocationIdentifier)[last()]'    , 'NVARCHAR(max)'),
                @StreetName                         =                       X.value('(/ns1:AddressPostal/ns3:StreetName)[last()]'                           , 'NVARCHAR(max)') ,
                @StreetNameForAddressingName        =                       X.value('(/ns1:AddressPostal/ns4:StreetNameForAddressingName)[last()]'          , 'NVARCHAR(max)'),
                @StreetBuildingIdentifier_Postal    = dbo.TrimAddressString(X.value('(/ns1:AddressPostal/ns2:StreetBuildingIdentifier)[last()]'             , 'NVARCHAR(max)')),
                @FloorIdentifier                    = dbo.TrimAddressString(X.value('(/ns1:AddressPostal/ns2:FloorIdentifier)[last()]'                      , 'NVARCHAR(max)')),
                @SuiteIdentifier                    = dbo.TrimAddressString(X.value('(/ns1:AddressPostal/ns2:SuiteIdentifier)[last()]'                      , 'NVARCHAR(max)')),
                @DistrictSubdivisionIdentifier      =                       X.value('(/ns1:AddressPostal/ns3:DistrictSubdivisionIdentifier)[last()]'        , 'NVARCHAR(max)'),
                @PostOfficeBoxIdentifier            =                       X.value('(/ns1:AddressPostal/ns5:PostOfficeBoxIdentifier)[last()]'              , 'NVARCHAR(max)'),
                @PostCodeIdentifier                 =                       X.value('(/ns1:AddressPostal/ns3:PostCodeIdentifier)[last()]'                   , 'int'),
                @DistrictName                       =                       X.value('(/ns1:AddressPostal/ns3:DistrictName)[last()]'                         , 'NVARCHAR(max)'),
                @CountryIdentificationCode          =                       X.value('(/ns1:AddressPostal/ns2:CountryIdentificationCode)[last()]'            , 'NVARCHAR(max)')
            FROM 
                (SELECT X.query('/ns0:DanskAdresse/ns1:AddressComplete/ns1:AddressPostal') FROM @DanskAdresseTable) AS tmp_AddressPostalTable(X);

            -- Greenlandic address
            DECLARE @GroenlandskAdresseTable TABLE(X XML);
            WITH XMLNAMESPACES ('urn:oio:sagdok:person:1.0.0' as ns0)
            INSERT INTO @GroenlandskAdresseTable SELECT X.query('/ns0:RegisterOplysning/ns0:CprBorger/ns0:FolkeregisterAdresse/ns0:GroenlandAdresse') FROM @RegisterOplysningTable;
        END

        IF (SELECT LEN(CAST(X AS NVARCHAR(MAX))) FROM @GroenlandskAdresseTable) > 0
        BEGIN
            WITH XMLNAMESPACES (
                'urn:oio:sagdok:person:1.0.0' as ns0,
                'urn:oio:sagdok:2.0.0' AS ns1)
            SELECT 
                @AddressType             = 'G',
                @NoteTekst_DanskAdresse  = X.value('(/ns0:GroenlandAdresse/ns1:NoteTekst)[last()]'               , 'NVARCHAR(max)'),
                @UkendtAdresseIndikator  = X.value('(/ns0:GroenlandAdresse/ns0:UkendtAdresseIndikator)[last()]'  , 'bit'),
                @SpecielVejkodeIndikator = X.value('(/ns0:GroenlandAdresse/ns0:SpecielVejkodeIndikator)[last()]' , 'bit')
            FROM @GroenlandskAdresseTable;

            -- AddressCompleteGreenland
            WITH XMLNAMESPACES (
                'urn:oio:sagdok:person:1.0.0' as ns0,
                'http://rep.oio.dk/cpr.dk/xml/schemas/2008/05/01/' AS ns1,
                'http://rep.oio.dk/cpr.dk/xml/schemas/core/2005/03/18/' as ns2,
                'http://rep.oio.dk/ebxml/xml/schemas/dkcc/2003/02/13/' as ns3,
                'http://rep.oio.dk/ebxml/xml/schemas/dkcc/2005/03/15/' as ns4)
            SELECT
                @MunicipalityCode                   =                        X.value('(/ns1:AddressCompleteGreenland/ns2:MunicipalityCode)[last()]'                  , 'int'),
                @StreetCode                         =                        X.value('(/ns1:AddressCompleteGreenland/ns2:StreetCode)[last()]'                        , 'int'),
                @MailDeliverySublocationIdentifier  =                        X.value('(/ns1:AddressCompleteGreenland/ns3:MailDeliverySublocationIdentifier)[last()]' , 'NVARCHAR(max)'),
                @StreetName                         =                        X.value('(/ns1:AddressCompleteGreenland/ns4:StreetName)[last()]'                        , 'NVARCHAR(max)'),
                @StreetNameForAddressingName        =                        X.value('(/ns1:AddressCompleteGreenland/ns2:StreetCode)[last()]'                        , 'NVARCHAR(max)'),
                @StreetBuildingIdentifier           = dbo.TrimAddressString( X.value('(/ns1:AddressCompleteGreenland/ns3:StreetBuildingIdentifier)[last()]'          , 'NVARCHAR(max)')),
                @GreenlandBuildingIdentifier        = dbo.TrimAddressString( X.value('(/ns1:AddressCompleteGreenland/ns1:GreenlandBuildingIdentifier)[last()]'       , 'NVARCHAR(max)')),
                @SuiteIdentifier                    = dbo.TrimAddressString( X.value('(/ns1:AddressCompleteGreenland/ns3:SuiteIdentifier)[last()]'                   , 'NVARCHAR(max)')),
                @FloorIdentifier                    = dbo.TrimAddressString( X.value('(/ns1:AddressCompleteGreenland/ns3:FloorIdentifier)[last()]'                   , 'NVARCHAR(max)')),
                @DistrictSubdivisionIdentifier      =                        X.value('(/ns1:AddressCompleteGreenland/ns4:DistrictSubdivisionIdentifier)[last()]'     , 'NVARCHAR(max)'),
                @PostCodeIdentifier                 =                        X.value('(/ns1:AddressCompleteGreenland/ns4:PostCodeIdentifier)[last()]'                , 'NVARCHAR(max)'),
                @DistrictName                       =                        X.value('(/ns1:AddressCompleteGreenland/ns4:DistrictName)[last()]'                      , 'NVARCHAR(max)'),
                @CountryIdentificationCode          =                        X.value('(/ns1:AddressCompleteGreenland/ns3:CountryIdentificationCode)[last()]'         , 'NVARCHAR(max)')
            FROM 
                (SELECT X.query('/ns0:GroenlandAdresse/ns1:AddressCompleteGreenland') FROM @GroenlandskAdresseTable) AS tmp_AddressPostalTable(X);
        END

        -- World address
        DECLARE @VerdenAdresseTable TABLE(X XML);
        WITH XMLNAMESPACES ('urn:oio:sagdok:person:1.0.0' as ns0)
        INSERT INTO @VerdenAdresseTable SELECT X.query('/ns0:RegisterOplysning/ns0:CprBorger/ns0:FolkeregisterAdresse/ns0:VerdenAdresse') FROM @RegisterOplysningTable;

        IF (SELECT LEN(CAST(X AS NVARCHAR(MAX))) FROM @VerdenAdresseTable) > 0
        BEGIN
            WITH XMLNAMESPACES (
                'urn:oio:sagdok:person:1.0.0' as ns0,
                'http://rep.oio.dk/cpr.dk/xml/schemas/2008/05/01/' as ns1,
                'http://rep.oio.dk/ebxml/xml/schemas/dkcc/2005/05/19/' as ns2,
                'http://rep.oio.dk/capevo.dk/xml/schemas/2007/08/01/' as ns3,
                'http://rep.oio.dk/ebxml/xml/schemas/dkcc/2003/02/13/' as ns4
                )
            SELECT
                @AddressType                    = 'V',
                @LocationDescriptionText        = X.value('(ns1:ForeignAddressStructure/ns3:LocationDescriptionText)[last()]'       , 'NVARCHAR(max)'),
                @PostalAddressFirstLineText     = X.value('(ns1:ForeignAddressStructure/ns2:PostalAddressFirstLineText)[last()]'    , 'NVARCHAR(max)'),
                @PostalAddressSecondLineText    = X.value('(ns1:ForeignAddressStructure/ns2:PostalAddressSecondLineText)[last()]'   , 'NVARCHAR(max)'),
                @PostalAddressThirdLineText     = X.value('(ns1:ForeignAddressStructure/ns2:PostalAddressThirdLineText)[last()]'    , 'NVARCHAR(max)'),
                @PostalAddressFourthLineText    = X.value('(ns1:ForeignAddressStructure/ns2:PostalAddressFourthLineText)[last()]'   , 'NVARCHAR(max)'),
                @PostalAddressFifthLineText     = X.value('(ns1:ForeignAddressStructure/ns2:PostalAddressFifthLineText)[last()]'    , 'NVARCHAR(max)'),
                @CountryIdentificationCode      = X.value('(/ns1:ForeignAddressStructure/ns4:CountryIdentificationCode)[last()]'    , 'NVARCHAR(max)')
            FROM
                (SELECT X.query('/ns0:VerdenAdresse/ns1:ForeignAddressStructure') FROM @VerdenAdresseTable) AS tmp_AddressPostalTable(X);
        END
    END
    

    -----------------------------
    -- INSERT OR UPDATE RECORD --
    -----------------------------
    IF EXISTS (SELECT UUID FROM PersonSearchCache WHERE UUID = @UUID)
    BEGIN			
        UPDATE PersonSearchCache
        SET 
            -- Root fields
            PersonRegistrationId = @PersonRegistrationId, LivscyklusKode = @LivscyklusKode,	

            -- Egenskab fields
            AddressingName = @AddressingName, NickName = @NickName, Note = @Note, 
            PersonGivenName = @PersonGivenName, PersonMiddleName = @PersonMiddleName, PersonSurnameName = @PersonSurnameName, 
            PersonGenderCode = @PersonGenderCode, Birthdate = @Birthdate,

            -- CprBorger
            UserInterfaceKeyText = @UserInterfaceKeyText, PersonCivilRegistrationIdentifier = @PersonCivilRegistrationIdentifier,
            PersonNummerGyldighedStatusIndikator = @PersonNummerGyldighedStatusIndikator, PersonNationalityCode = @PersonNationalityCode,
            NavneAdresseBeskyttelseIndikator = @NavneAdresseBeskyttelseIndikator, TelefonNummerBeskyttelseIndikator = @TelefonNummerBeskyttelseIndikator, ForskerBeskyttelseIndikator = @ForskerBeskyttelseIndikator,
            
            -- CprBorger - after address
            AdresseNoteTekst = @AdresseNoteTekst, FolkekirkeMedlemIndikator = @FolkekirkeMedlemIndikator, 

            -- FolkeregisterAdresse
            AddressType = @AddressType,
            NoteTekst_DanskAdresse = @NoteTekst_DanskAdresse, UkendtAdresseIndikator = @UkendtAdresseIndikator,
            SpecielVejkodeIndikator = @SpecielVejkodeIndikator, PostDistriktTekst = @PostDistriktTekst,

            -- AddressAccess
            MunicipalityCode = @MunicipalityCode, StreetCode = @StreetCode, StreetBuildingIdentifier = @StreetBuildingIdentifier,

            -- AddressPostal
            MailDeliverySublocationIdentifier = @MailDeliverySublocationIdentifier, StreetName = @StreetName, StreetNameForAddressingName = @StreetNameForAddressingName,
            StreetBuildingIdentifier_Postal = @StreetBuildingIdentifier_Postal, FloorIdentifier = @FloorIdentifier, SuiteIdentifier = @SuiteIdentifier,
            DistrictSubdivisionIdentifier = @DistrictSubdivisionIdentifier, PostOfficeBoxIdentifier = @PostOfficeBoxIdentifier,
            PostCodeIdentifier = @PostCodeIdentifier, DistrictName = @DistrictName,
            CountryIdentificationCode = @CountryIdentificationCode,

            -- Greenlandic address
            GreenlandBuildingIdentifier = @GreenlandBuildingIdentifier,

            -- World address
            LocationDescriptionText = @LocationDescriptionText,
            PostalAddressFirstLineText = @PostalAddressFirstLineText, PostalAddressSecondLineText = @PostalAddressSecondLineText, PostalAddressThirdLineText = @PostalAddressThirdLineText,
            PostalAddressFourthLineText = @PostalAddressFourthLineText, PostalAddressFifthLineText = @PostalAddressFifthLineText

        WHERE 
            UUID = @UUID
    END
    ELSE
    BEGIN
        INSERT INTO PersonSearchCache (
            -- Root fields
            UUID, PersonRegistrationId, LivscyklusKode, 
            
            -- Egenskab fields
            AddressingName, NickName, Note, 
            PersonGivenName, PersonMiddleName, PersonSurnameName, 
            PersonGenderCode, Birthdate, 
            
            -- CprBorger
            UserInterfaceKeyText, PersonCivilRegistrationIdentifier,
            PersonNummerGyldighedStatusIndikator, PersonNationalityCode,
            NavneAdresseBeskyttelseIndikator, TelefonNummerBeskyttelseIndikator, ForskerBeskyttelseIndikator,
            
            -- CprBorger - after address
            AdresseNoteTekst, FolkekirkeMedlemIndikator,

            -- FolkeregisterAdresse
            AddressType,
            NoteTekst_DanskAdresse, UkendtAdresseIndikator,
            SpecielVejkodeIndikator, PostDistriktTekst,

            -- AddressAccess
            MunicipalityCode, StreetCode, StreetBuildingIdentifier,

            -- AddressPostal
            MailDeliverySublocationIdentifier, StreetName, StreetNameForAddressingName,
            StreetBuildingIdentifier_Postal, FloorIdentifier, SuiteIdentifier,
            DistrictSubdivisionIdentifier, PostOfficeBoxIdentifier,
            PostCodeIdentifier, DistrictName,
            CountryIdentificationCode,

            -- Greenlandic address
            GreenlandBuildingIdentifier,

            -- World address
            LocationDescriptionText,
            PostalAddressFirstLineText, PostalAddressSecondLineText, PostalAddressThirdLineText,
            PostalAddressFourthLineText, PostalAddressFifthLineText
            )

        VALUES (
            -- Root fields
            @UUID, @PersonRegistrationId, @LivscyklusKode, 

            -- Egenskab fields
            @AddressingName, @NickName, @Note, 
            @PersonGivenName, @PersonMiddleName, @PersonSurnameName, 
            @PersonGenderCode, @Birthdate, 

            -- CprBorger
            @UserInterfaceKeyText, @PersonCivilRegistrationIdentifier,
            @PersonNummerGyldighedStatusIndikator, @PersonNationalityCode,
            @NavneAdresseBeskyttelseIndikator, @TelefonNummerBeskyttelseIndikator, @ForskerBeskyttelseIndikator,
            
            -- CprBorger - after address
            @AdresseNoteTekst, @FolkekirkeMedlemIndikator,

            -- FolkeregisterAdresse
            @AddressType,
            @NoteTekst_DanskAdresse, @UkendtAdresseIndikator,
            @SpecielVejkodeIndikator, @PostDistriktTekst,

            -- AddressAccess
            @MunicipalityCode, @StreetCode, @StreetBuildingIdentifier,

            -- AddressPostal
            @MailDeliverySublocationIdentifier, @StreetName, @StreetNameForAddressingName,
            @StreetBuildingIdentifier_Postal, @FloorIdentifier, @SuiteIdentifier,
            @DistrictSubdivisionIdentifier, @PostOfficeBoxIdentifier,
            @PostCodeIdentifier, @DistrictName,
            @CountryIdentificationCode,

            -- Greenlandic address
            @GreenlandBuildingIdentifier,

            -- World address
            @LocationDescriptionText,
            @PostalAddressFirstLineText, @PostalAddressSecondLineText, @PostalAddressThirdLineText,
            @PostalAddressFourthLineText, @PostalAddressFifthLineText
            )
    END
END

GO