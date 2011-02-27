SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PersonStatusType](
	[PersonStatusTypeCode] [char](2) NOT NULL,
	[PersonStatusName] [nvarchar](90) NOT NULL,
 CONSTRAINT [PK_PersonStatusType] PRIMARY KEY CLUSTERED 
(
	[PersonStatusTypeCode] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RelationshipType](
	[RelationshipTypeId] [int] NOT NULL,
	[ForwardName] [varchar](50) NOT NULL,
	[BackwardName] [varchar](50) NOT NULL,
 CONSTRAINT [PK_RelationshipType] PRIMARY KEY CLUSTERED 
(
	[RelationshipTypeId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Country](
	[Alpha2Code] [varchar](2) NOT NULL,
	[Alpha3Code] [varchar](3) NOT NULL,
	[NumericCode] [int] NOT NULL,
	[CountryName] [nvarchar](60) NOT NULL,
	[Description] [nvarchar](50) NULL,
	[DanishCountryName] [nvarchar](60) NOT NULL,
	[DanishCountryName2] [nvarchar](50) NULL,
	[KmdCode] [int] NULL,
	[KmdCode2] [int] NULL,
	[KmdCode3] [int] NULL,
 CONSTRAINT [PK_Country] PRIMARY KEY CLUSTERED 
(
	[Alpha2Code] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
EXEC sys.sp_addextendedproperty @name=N'Notes', @value=N'This is a list of iso3166 Standard Country Codes' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Country'
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Function [dbo].[Equals]
(
	@Value1 sql_variant,
	@Value2 sql_variant
)
RETURNS BIT
AS
BEGIN
	IF @Value1 IS NULL AND @Value2 IS NULL 
		RETURN 1
	ELSE IF @Value1 IS NULL OR @Value2 IS NULL 
		RETURN 0 
	ELSE IF @Value1 = @Value2 
		RETURN 1
	
	RETURN 0
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubscriptionType](
	[SubscriptionTypeId] [int] NOT NULL,
	[TypeName] [varchar](50) NOT NULL,
 CONSTRAINT [PK_SubscriptionType] PRIMARY KEY CLUSTERED 
(
	[SubscriptionTypeId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Gender](
	[GenderId] [int] NOT NULL,
	[GenderName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Gender] PRIMARY KEY CLUSTERED 
(
	[GenderId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChannelType](
	[ChannelTypeId] [int] NOT NULL,
	[Name] [nvarchar](50) NULL,
 CONSTRAINT [PK_ChannelType] PRIMARY KEY CLUSTERED 
(
	[ChannelTypeId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Function [dbo].[IsBirthdateEvent]
(
		@Now DateTime,
		@BirthDate DateTime,
		@AgeYears INT,
		@PriorDays INT
)
RETURNS BIT
AS
BEGIN
	IF
	(	
		-- Exact age match
		@AgeYears IS NOT NULL 
		AND dateadd (day, @PriorDays, @Now) = dateadd(year, @AgeYears, @BirthDate)
	)
	OR
	(
		-- Any age match
		@AgeYears IS NULL 
		AND DATEPART(DAY,DATEADD(day, @PriorDays, @Now)) = DATEPART(DAY,@BirthDate)
		AND DATEPART(MONTH,DATEADD(day, @PriorDays, @Now)) = DATEPART(MONTH,@BirthDate)
	)
		RETURN 1
		
	RETURN 0
END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[EmulatePersonDataUpdate]
(
	@MaxPersons INT
)

AS
	CREATE TABLE #PersonId (PersonId UNIQUEIDENTIFIER)
	DECLARE @SQL VARCHAR(MAX)
	SET @SQL = 'INSERT INTO #PersonId (PersonId) SELECT TOP ' + CAST (@MaxPersons AS VARCHAR(20)) + ' PersonId FROM Person'
	EXEC (@SQL)

	DECLARE @FakeBirthdate DATETIME
	SET @FakeBirthdate = GETDATE()
	SET @FakeBirthdate = DATEADD(hour, - (DATEPART(hour,@FakeBirthdate)),@FakeBirthdate)
	SET @FakeBirthdate = DATEADD(minute, - (DATEPART(minute,@FakeBirthdate)),@FakeBirthdate)
	SET @FakeBirthdate = DATEADD(second, - (DATEPART(second,@FakeBirthdate)),@FakeBirthdate)
	SET @FakeBirthdate = DATEADD(millisecond, - (DATEPART(millisecond,@FakeBirthdate)),@FakeBirthdate)
	
	UPDATE P
	SET BirthDate = DATEADD(DAY, 1, ISNULL(BirthDate, @FakeBirthdate))
	FROM Person P
	INNER JOIN #PersonId TMP ON P.PersonId = TMP.PersonId


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DetailLevel](
	[DetailLevelId] [int] NOT NULL,
	[Name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_DetailLevel] PRIMARY KEY CLUSTERED 
(
	[DetailLevelId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MaritalStatusType](
	[MaritalStatusTypeId] [int] NOT NULL,
	[MaritalStatusName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_MaritalStatus] PRIMARY KEY CLUSTERED 
(
	[MaritalStatusTypeId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OperationType](
	[OperationTypeId] [int] NOT NULL,
	[OperationName] [varchar](50) NOT NULL,
	[SourceTableName] [varchar](50) NOT NULL,
	[OldTableName] [varchar](50) NULL,
 CONSTRAINT [PK_OperationType] PRIMARY KEY CLUSTERED 
(
	[OperationTypeId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogType](
	[LogTypeId] [int] NOT NULL,
	[Name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_LogType] PRIMARY KEY CLUSTERED 
(
	[LogTypeId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Municipality](
	[MunicipalityCode] [varchar](4) NOT NULL,
	[MunicipalityName] [nvarchar](24) NOT NULL,
 CONSTRAINT [PK_Municipality_1] PRIMARY KEY CLUSTERED 
(
	[MunicipalityCode] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DataProviderType](
	[DataProviderTypeId] [int] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Enabled] [bit] NOT NULL CONSTRAINT [DF_Provider_Enabled]  DEFAULT ((1)),
	[TypeName] [varchar](250) NOT NULL,
	[IsExternal] [bit] NOT NULL,
 CONSTRAINT [PK_DataProviderType] PRIMARY KEY CLUSTERED 
(
	[DataProviderTypeId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AddressStatus](
	[AddressStatusId] [int] NOT NULL,
	[AddressStatusName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_AddressStatus] PRIMARY KEY CLUSTERED 
(
	[AddressStatusId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Application](
	[ApplicationId] [uniqueidentifier] NOT NULL CONSTRAINT [DF_Application_ApplicationId]  DEFAULT (newid()),
	[Name] [nvarchar](100) NOT NULL,
	[Token] [varchar](50) NOT NULL,
	[RegistrationDate] [datetime] NOT NULL,
	[IsApproved] [bit] NOT NULL CONSTRAINT [DF_Application_IsApproved]  DEFAULT ((0)),
	[ApprovedDate] [datetime] NULL,
 CONSTRAINT [PK_Application] PRIMARY KEY CLUSTERED 
(
	[ApplicationId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Person](
	[PersonId] [uniqueidentifier] NOT NULL CONSTRAINT [DF_Person_PersonId]  DEFAULT (newid()),
	[DetailLevelId] [int] NULL,
	[NationalityCountryAlpha2Code] [varchar](2) NULL,
	[PersonNumber] [char](10) NOT NULL,
	[FirstName] [nvarchar](34) NULL,
	[MiddleName] [nvarchar](34) NULL,
	[LastName] [nvarchar](34) NULL,
	[NameForAddressing] [nvarchar](34) NULL,
	[BirthDate] [datetime] NULL,
	[BirthDateUncertaintyIndicator] [bit] NULL,
	[DeathDate] [datetime] NULL,
	[DeathDateUncertaintyIndicator] [bit] NULL,
	[MaritalStatusTypeId] [int] NULL,
	[PersonStatusTypeCode] [char](2) NULL,
	[PersonStatusDate] [datetime] NULL,
	[GenderId] [int] NULL,
	[PersonProtectionIndicator] [bit] NULL,
	[PersonProtectionIndicatorStartDate] [datetime] NULL,
	[PersonProtectionIndicatorEndDate] [datetime] NULL,
	[ModifiedDate] [datetime] NOT NULL,
	[IsTestPerson] [bit] NOT NULL CONSTRAINT [DF_Person_IsTestPerson]  DEFAULT ((0)),
 CONSTRAINT [PK_Person_1] PRIMARY KEY CLUSTERED 
(
	[PersonId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER [dbo].[Person_Delete]
 ON [dbo].[Person] FOR Delete
 AS 
 INSERT INTO [PersonHistory]
 (OperationTypeId, ChangeDate, PersonId, NationalityCountryAlpha2Code, PersonNumber, FirstName, MiddleName, LastName, NameForAddressing, BirthDate, BirthDateUncertaintyIndicator, DeathDate, DeathDateUncertaintyIndicator, MaritalStatusTypeId, PersonStatusDate, GenderId, PersonProtectionIndicator, PersonProtectionIndicatorStartDate, PersonProtectionIndicatorEndDate, IsTestPerson)
 SELECT 
3, getdate(),PersonId,NationalityCountryAlpha2Code,PersonNumber,FirstName,MiddleName,LastName,NameForAddressing,BirthDate,BirthDateUncertaintyIndicator,DeathDate,DeathDateUncertaintyIndicator,MaritalStatusTypeId,PersonStatusDate,GenderId,PersonProtectionIndicator,PersonProtectionIndicatorStartDate,PersonProtectionIndicatorEndDate,IsTestPerson
 FROM DELETED


GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER [dbo].[Person_Insert]
 ON [dbo].[Person] FOR Insert
 AS 
 INSERT INTO [PersonHistory]
 (OperationTypeId, ChangeDate, PersonId, NationalityCountryAlpha2Code, PersonNumber, FirstName, MiddleName, LastName, NameForAddressing, BirthDate, BirthDateUncertaintyIndicator, DeathDate, DeathDateUncertaintyIndicator, MaritalStatusTypeId, PersonStatusDate, GenderId, PersonProtectionIndicator, PersonProtectionIndicatorStartDate, PersonProtectionIndicatorEndDate, IsTestPerson)
 SELECT 
1, getdate(),PersonId,NationalityCountryAlpha2Code,PersonNumber,FirstName,MiddleName,LastName,NameForAddressing,BirthDate,BirthDateUncertaintyIndicator,DeathDate,DeathDateUncertaintyIndicator,MaritalStatusTypeId,PersonStatusDate,GenderId,PersonProtectionIndicator,PersonProtectionIndicatorStartDate,PersonProtectionIndicatorEndDate,IsTestPerson
 FROM INSERTED


GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE TRIGGER [dbo].[Person_Update]
 ON [dbo].[Person] FOR Update
 AS 
 IF ( 
 dbo.Equals( (SELECT TOP 1 [PersonId] FROM INSERTED) , (SELECT TOP 1 [PersonId] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [NationalityCountryAlpha2Code] FROM INSERTED) , (SELECT TOP 1 [NationalityCountryAlpha2Code] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [PersonNumber] FROM INSERTED) , (SELECT TOP 1 [PersonNumber] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [FirstName] FROM INSERTED) , (SELECT TOP 1 [FirstName] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [MiddleName] FROM INSERTED) , (SELECT TOP 1 [MiddleName] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [LastName] FROM INSERTED) , (SELECT TOP 1 [LastName] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [NameForAddressing] FROM INSERTED) , (SELECT TOP 1 [NameForAddressing] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [BirthDate] FROM INSERTED) , (SELECT TOP 1 [BirthDate] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [BirthDateUncertaintyIndicator] FROM INSERTED) , (SELECT TOP 1 [BirthDateUncertaintyIndicator] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [DeathDate] FROM INSERTED) , (SELECT TOP 1 [DeathDate] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [DeathDateUncertaintyIndicator] FROM INSERTED) , (SELECT TOP 1 [DeathDateUncertaintyIndicator] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [MaritalStatusTypeId] FROM INSERTED) , (SELECT TOP 1 [MaritalStatusTypeId] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [PersonStatusDate] FROM INSERTED) , (SELECT TOP 1 [PersonStatusDate] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [GenderId] FROM INSERTED) , (SELECT TOP 1 [GenderId] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [PersonProtectionIndicator] FROM INSERTED) , (SELECT TOP 1 [PersonProtectionIndicator] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [PersonProtectionIndicatorStartDate] FROM INSERTED) , (SELECT TOP 1 [PersonProtectionIndicatorStartDate] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [PersonProtectionIndicatorEndDate] FROM INSERTED) , (SELECT TOP 1 [PersonProtectionIndicatorEndDate] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [IsTestPerson] FROM INSERTED) , (SELECT TOP 1 [IsTestPerson] FROM DELETED)) = 0 
 ) 
 INSERT INTO [PersonHistory]
 (OperationTypeId, ChangeDate, PersonId, NationalityCountryAlpha2Code, PersonNumber, FirstName, MiddleName, LastName, NameForAddressing, BirthDate, BirthDateUncertaintyIndicator, DeathDate, DeathDateUncertaintyIndicator, MaritalStatusTypeId, PersonStatusDate, GenderId, PersonProtectionIndicator, PersonProtectionIndicatorStartDate, PersonProtectionIndicatorEndDate, IsTestPerson)
 SELECT 
2, getdate(),PersonId,NationalityCountryAlpha2Code,PersonNumber,FirstName,MiddleName,LastName,NameForAddressing,BirthDate,BirthDateUncertaintyIndicator,DeathDate,DeathDateUncertaintyIndicator,MaritalStatusTypeId,PersonStatusDate,GenderId,PersonProtectionIndicator,PersonProtectionIndicatorStartDate,PersonProtectionIndicatorEndDate,IsTestPerson
 FROM INSERTED


GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Specifies whether the birth date is uncertain, ie it has not been possible to verify the correctness of the date.

Optional.
0: date of birth is safe
1: The birth date is uncertain


' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Person', @level2type=N'COLUMN',@level2name=N'BirthDateUncertaintyIndicator'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Specifies whether the death date is uncertain, ie it has not been possible to verify the correctness of the date.

Optional.
0: date of death is safe
1: The death date is uncertain' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Person', @level2type=N'COLUMN',@level2name=N'DeathDateUncertaintyIndicator'
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Address](
	[PersonId] [uniqueidentifier] NOT NULL,
	[AddressStatusId] [int] NULL,
	[CountryAlpha2Code] [varchar](2) NULL,
	[MunicipalityCode] [varchar](4) NULL,
	[StreetCode] [varchar](4) NULL,
	[StreetName] [nvarchar](50) NULL,
	[StreetNameForAddressing] [nvarchar](20) NULL,
	[StreetBuildingIdentifier] [varchar](4) NULL,
	[FloorIdentifier] [varchar](2) NULL,
	[SuiteIdentifier] [nvarchar](4) NULL,
	[MailDeliverSubLocationIdentifier] [nvarchar](34) NULL,
	[DistrictSubDivisionIdentifier] [nvarchar](34) NULL,
	[PostBoxIdentifier] [nvarchar](4) NULL,
	[PostCode] [varchar](10) NULL,
	[PostDistrictName] [nvarchar](20) NULL,
	[CareOfName] [nvarchar](50) NULL,
	[Line1] [nvarchar](50) NULL,
	[Line2] [nvarchar](50) NULL,
	[Line3] [nvarchar](50) NULL,
	[Line4] [nvarchar](50) NULL,
	[Line5] [nvarchar](50) NULL,
	[Line6] [nvarchar](50) NULL,
	[LocationDescription] [nvarchar](50) NULL,
 CONSTRAINT [PK_Address] PRIMARY KEY CLUSTERED 
(
	[PersonId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER [dbo].[Address_Delete]
 ON [dbo].[Address] FOR Delete
 AS 
 INSERT INTO [AddressHistory]
 (OperationTypeId, ChangeDate, PersonId, AddressStatusId, CountryAlpha2Code, MunicipalityCode, StreetCode, StreetName, StreetNameForAddressing, StreetBuildingIdentifier, FloorIdentifier, SuiteIdentifier, MailDeliverSubLocationIdentifier, DistrictSubDivisionIdentifier, PostBoxIdentifier, PostCode, PostDistrictName, CareOfName, Line1, Line2, Line3, Line4, Line5, Line6, LocationDescription)
 SELECT 
3, getdate(),PersonId,AddressStatusId,CountryAlpha2Code,MunicipalityCode,StreetCode,StreetName,StreetNameForAddressing,StreetBuildingIdentifier,FloorIdentifier,SuiteIdentifier,MailDeliverSubLocationIdentifier,DistrictSubDivisionIdentifier,PostBoxIdentifier,PostCode,PostDistrictName,CareOfName,Line1,Line2,Line3,Line4,Line5,Line6,LocationDescription
 FROM DELETED


GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER [dbo].[Address_Insert]
 ON [dbo].[Address] FOR Insert
 AS 
 INSERT INTO [AddressHistory]
 (OperationTypeId, ChangeDate, PersonId, AddressStatusId, CountryAlpha2Code, MunicipalityCode, StreetCode, StreetName, StreetNameForAddressing, StreetBuildingIdentifier, FloorIdentifier, SuiteIdentifier, MailDeliverSubLocationIdentifier, DistrictSubDivisionIdentifier, PostBoxIdentifier, PostCode, PostDistrictName, CareOfName, Line1, Line2, Line3, Line4, Line5, Line6, LocationDescription)
 SELECT 
1, getdate(),PersonId,AddressStatusId,CountryAlpha2Code,MunicipalityCode,StreetCode,StreetName,StreetNameForAddressing,StreetBuildingIdentifier,FloorIdentifier,SuiteIdentifier,MailDeliverSubLocationIdentifier,DistrictSubDivisionIdentifier,PostBoxIdentifier,PostCode,PostDistrictName,CareOfName,Line1,Line2,Line3,Line4,Line5,Line6,LocationDescription
 FROM INSERTED


GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE TRIGGER [dbo].[Address_Update]
 ON [dbo].[Address] FOR Update
 AS 
 IF ( 
 dbo.Equals( (SELECT TOP 1 [PersonId] FROM INSERTED) , (SELECT TOP 1 [PersonId] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [AddressStatusId] FROM INSERTED) , (SELECT TOP 1 [AddressStatusId] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [CountryAlpha2Code] FROM INSERTED) , (SELECT TOP 1 [CountryAlpha2Code] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [MunicipalityCode] FROM INSERTED) , (SELECT TOP 1 [MunicipalityCode] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [StreetCode] FROM INSERTED) , (SELECT TOP 1 [StreetCode] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [StreetName] FROM INSERTED) , (SELECT TOP 1 [StreetName] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [StreetNameForAddressing] FROM INSERTED) , (SELECT TOP 1 [StreetNameForAddressing] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [StreetBuildingIdentifier] FROM INSERTED) , (SELECT TOP 1 [StreetBuildingIdentifier] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [FloorIdentifier] FROM INSERTED) , (SELECT TOP 1 [FloorIdentifier] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [SuiteIdentifier] FROM INSERTED) , (SELECT TOP 1 [SuiteIdentifier] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [MailDeliverSubLocationIdentifier] FROM INSERTED) , (SELECT TOP 1 [MailDeliverSubLocationIdentifier] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [DistrictSubDivisionIdentifier] FROM INSERTED) , (SELECT TOP 1 [DistrictSubDivisionIdentifier] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [PostBoxIdentifier] FROM INSERTED) , (SELECT TOP 1 [PostBoxIdentifier] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [PostCode] FROM INSERTED) , (SELECT TOP 1 [PostCode] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [PostDistrictName] FROM INSERTED) , (SELECT TOP 1 [PostDistrictName] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [CareOfName] FROM INSERTED) , (SELECT TOP 1 [CareOfName] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [Line1] FROM INSERTED) , (SELECT TOP 1 [Line1] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [Line2] FROM INSERTED) , (SELECT TOP 1 [Line2] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [Line3] FROM INSERTED) , (SELECT TOP 1 [Line3] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [Line4] FROM INSERTED) , (SELECT TOP 1 [Line4] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [Line5] FROM INSERTED) , (SELECT TOP 1 [Line5] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [Line6] FROM INSERTED) , (SELECT TOP 1 [Line6] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [LocationDescription] FROM INSERTED) , (SELECT TOP 1 [LocationDescription] FROM DELETED)) = 0 
 ) 
 INSERT INTO [AddressHistory]
 (OperationTypeId, ChangeDate, PersonId, AddressStatusId, CountryAlpha2Code, MunicipalityCode, StreetCode, StreetName, StreetNameForAddressing, StreetBuildingIdentifier, FloorIdentifier, SuiteIdentifier, MailDeliverSubLocationIdentifier, DistrictSubDivisionIdentifier, PostBoxIdentifier, PostCode, PostDistrictName, CareOfName, Line1, Line2, Line3, Line4, Line5, Line6, LocationDescription)
 SELECT 
2, getdate(),PersonId,AddressStatusId,CountryAlpha2Code,MunicipalityCode,StreetCode,StreetName,StreetNameForAddressing,StreetBuildingIdentifier,FloorIdentifier,SuiteIdentifier,MailDeliverSubLocationIdentifier,DistrictSubDivisionIdentifier,PostBoxIdentifier,PostCode,PostDistrictName,CareOfName,Line1,Line2,Line3,Line4,Line5,Line6,LocationDescription
 FROM INSERTED


GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Numeric identification (da: house number ~ ''husnummer'') including an optional letter, which identifies a certain access to a building, a plot/piece of land or a plant etc. based on the named road or street which gives access hereto. The identifier is determined in increasing numerical and letter order along the road, normally with even numbers on the right side and odd numbers on the left side of the road.

<xsd:pattern value="([1-9]|[1-9][0-9]|[1-9][0-9]{2})|([1-9][A-Z]|[1-9][0-9][A-Z]|[1-9][0-9]{2}[A-Z])" />' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Address', @level2type=N'COLUMN',@level2name=N'StreetBuildingIdentifier'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Identification which describes the floor or level on which a specific entrance door, appartment or suite is placed, in the staircase refered to.

<xsd:pattern value="[1-9]|[1-9][0-9]|KL|ST|kl|st|k[2-9]|K[2-9]" />' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Address', @level2type=N'COLUMN',@level2name=N'FloorIdentifier'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Farm name, name of property building or residence or like. which are used as Additional postaddress' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Address', @level2type=N'COLUMN',@level2name=N'MailDeliverSubLocationIdentifier'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Town set out as a part of the official address identifying a particular path or for specific parts of a road(number intervals) As the city can used a real coty or another local place name' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Address', @level2type=N'COLUMN',@level2name=N'DistrictSubDivisionIdentifier'
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Relationship](
	[RelationshipId] [uniqueidentifier] NOT NULL CONSTRAINT [DF_Relationship_RelationshipId]  DEFAULT (newid()),
	[PersonId] [uniqueidentifier] NOT NULL,
	[RelatedPersonId] [uniqueidentifier] NOT NULL,
	[RelationshipTypeId] [int] NOT NULL,
	[RegistrationDate] [datetime] NOT NULL,
 CONSTRAINT [PK_Relationship] PRIMARY KEY CLUSTERED 
(
	[RelationshipId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER [dbo].[Relationship_Delete]
 ON [dbo].[Relationship] FOR Delete
 AS 
 INSERT INTO [RelationshipHistory]
 (OperationTypeId, ChangeDate, RelationshipId, PersonId, RelatedPersonId, RelationshipTypeId, RegistrationDate)
 SELECT 
3, getdate(),RelationshipId,PersonId,RelatedPersonId,RelationshipTypeId,RegistrationDate
 FROM DELETED


GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER [dbo].[Relationship_Insert]
 ON [dbo].[Relationship] FOR Insert
 AS 
 INSERT INTO [RelationshipHistory]
 (OperationTypeId, ChangeDate, RelationshipId, PersonId, RelatedPersonId, RelationshipTypeId, RegistrationDate)
 SELECT 
1, getdate(),RelationshipId,PersonId,RelatedPersonId,RelationshipTypeId,RegistrationDate
 FROM INSERTED


GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE TRIGGER [dbo].[Relationship_Update]
 ON [dbo].[Relationship] FOR Update
 AS 
 IF ( 
 dbo.Equals( (SELECT TOP 1 [RelationshipId] FROM INSERTED) , (SELECT TOP 1 [RelationshipId] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [PersonId] FROM INSERTED) , (SELECT TOP 1 [PersonId] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [RelatedPersonId] FROM INSERTED) , (SELECT TOP 1 [RelatedPersonId] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [RelationshipTypeId] FROM INSERTED) , (SELECT TOP 1 [RelationshipTypeId] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [RegistrationDate] FROM INSERTED) , (SELECT TOP 1 [RegistrationDate] FROM DELETED)) = 0 
 ) 
 INSERT INTO [RelationshipHistory]
 (OperationTypeId, ChangeDate, RelationshipId, PersonId, RelatedPersonId, RelationshipTypeId, RegistrationDate)
 SELECT 
2, getdate(),RelationshipId,PersonId,RelatedPersonId,RelationshipTypeId,RegistrationDate
 FROM INSERTED


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubscriptionPerson](
	[SubscriptionPersonId] [uniqueidentifier] NOT NULL CONSTRAINT [DF_SubscriptionPerson_SubscriptionPersonId]  DEFAULT (newid()),
	[SubscriptionId] [uniqueidentifier] NULL,
	[PersonId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_SubscriptionPerson] PRIMARY KEY CLUSTERED 
(
	[SubscriptionPersonId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [IX_SubscriptionPerson] UNIQUE NONCLUSTERED 
(
	[SubscriptionId] ASC,
	[PersonId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NotificationPerson](
	[NotificationPersonId] [uniqueidentifier] NOT NULL CONSTRAINT [DF_NotificationPerson_NotificationPersonId]  DEFAULT (newid()),
	[NotificationId] [uniqueidentifier] NOT NULL,
	[PersonId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_NotificationPerson] PRIMARY KEY CLUSTERED 
(
	[NotificationPersonId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Notification](
	[NotificationId] [uniqueidentifier] NOT NULL CONSTRAINT [DF_Notification_NotificationId]  DEFAULT (newid()),
	[SubscriptionId] [uniqueidentifier] NOT NULL,
	[NotificationDate] [datetime] NOT NULL,
	[CreatedDate] [datetime] NOT NULL CONSTRAINT [DF_Notification_CreatedDate]  DEFAULT (getdate()),
 CONSTRAINT [PK_Notification] PRIMARY KEY CLUSTERED 
(
	[NotificationId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Channel](
	[ChannelId] [uniqueidentifier] NOT NULL CONSTRAINT [DF_Channel_ChannelId]  DEFAULT (newid()),
	[ChannelTypeId] [int] NOT NULL,
	[SubscriptionId] [uniqueidentifier] NOT NULL,
	[Url] [varchar](250) NOT NULL,
 CONSTRAINT [PK_Channel] PRIMARY KEY CLUSTERED 
(
	[ChannelId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [IX_Channel] UNIQUE NONCLUSTERED 
(
	[SubscriptionId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BirthdateSubscription](
	[SubscriptionId] [uniqueidentifier] NOT NULL CONSTRAINT [DF_BirthdateSubscription_SubscriptionId]  DEFAULT (newid()),
	[AgeYears] [int] NULL,
	[PriorDays] [int] NOT NULL CONSTRAINT [DF_BirthdateSubscription_OffsetDays]  DEFAULT ((0)),
 CONSTRAINT [PK_BirthdateSubscription] PRIMARY KEY CLUSTERED 
(
	[SubscriptionId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DataSubscription](
	[SubscriptionId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_DataSubscription] PRIMARY KEY CLUSTERED 
(
	[SubscriptionId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GpacChannel](
	[ChannelId] [uniqueidentifier] NOT NULL,
	[SourceUri] [varchar](250) NOT NULL,
	[ObjectType] [int] NOT NULL,
	[NotifyType] [int] NOT NULL,
 CONSTRAINT [PK_GpacChannel] PRIMARY KEY CLUSTERED 
(
	[ChannelId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChildRelationship](
	[RelationshipId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_ChildRelationship] PRIMARY KEY CLUSTERED 
(
	[RelationshipId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TimedRelationship](
	[RelationshipId] [uniqueidentifier] NOT NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
 CONSTRAINT [PK_TimedRelationship] PRIMARY KEY CLUSTERED 
(
	[RelationshipId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER [dbo].[TimedRelationship_Delete]
 ON [dbo].[TimedRelationship] FOR Delete
 AS 
 INSERT INTO [TimedRelationshipHistory]
 (OperationTypeId, ChangeDate, RelationshipId, StartDate, EndDate)
 SELECT 
3, getdate(),RelationshipId,StartDate,EndDate
 FROM DELETED


GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER [dbo].[TimedRelationship_Insert]
 ON [dbo].[TimedRelationship] FOR Insert
 AS 
 INSERT INTO [TimedRelationshipHistory]
 (OperationTypeId, ChangeDate, RelationshipId, StartDate, EndDate)
 SELECT 
1, getdate(),RelationshipId,StartDate,EndDate
 FROM INSERTED


GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE TRIGGER [dbo].[TimedRelationship_Update]
 ON [dbo].[TimedRelationship] FOR Update
 AS 
 IF ( 
 dbo.Equals( (SELECT TOP 1 [RelationshipId] FROM INSERTED) , (SELECT TOP 1 [RelationshipId] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [StartDate] FROM INSERTED) , (SELECT TOP 1 [StartDate] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [EndDate] FROM INSERTED) , (SELECT TOP 1 [EndDate] FROM DELETED)) = 0 
 ) 
 INSERT INTO [TimedRelationshipHistory]
 (OperationTypeId, ChangeDate, RelationshipId, StartDate, EndDate)
 SELECT 
2, getdate(),RelationshipId,StartDate,EndDate
 FROM INSERTED


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BirthdateNotification](
	[NotificationId] [uniqueidentifier] NOT NULL,
	[AgeYears] [int] NULL,
	[PriorDays] [int] NOT NULL,
 CONSTRAINT [PK_BirthdateNotification] PRIMARY KEY CLUSTERED 
(
	[NotificationId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Subscription](
	[SubscriptionId] [uniqueidentifier] NOT NULL CONSTRAINT [DF_Subscription_SubscriptionId]  DEFAULT (newid()),
	[SubscriptionTypeId] [int] NOT NULL,
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[IsForAllPersons] [bit] NOT NULL CONSTRAINT [DF_Subscription_IsForAllPersons]  DEFAULT ((1)),
 CONSTRAINT [PK_Subscription] PRIMARY KEY CLUSTERED 
(
	[SubscriptionId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MaritalRelationship](
	[RelationshipId] [uniqueidentifier] NOT NULL,
	[MaritalStatusId] [int] NULL,
 CONSTRAINT [PK_MaritalRelationship] PRIMARY KEY CLUSTERED 
(
	[RelationshipId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER [dbo].[MaritalRelationship_Delete]
 ON [dbo].[MaritalRelationship] FOR Delete
 AS 
 INSERT INTO [MaritalRelationshipHistory]
 (OperationTypeId, ChangeDate, RelationshipId, MaritalStatusId)
 SELECT 
3, getdate(),RelationshipId,MaritalStatusId
 FROM DELETED


GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER [dbo].[MaritalRelationship_Insert]
 ON [dbo].[MaritalRelationship] FOR Insert
 AS 
 INSERT INTO [MaritalRelationshipHistory]
 (OperationTypeId, ChangeDate, RelationshipId, MaritalStatusId)
 SELECT 
1, getdate(),RelationshipId,MaritalStatusId
 FROM INSERTED


GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE TRIGGER [dbo].[MaritalRelationship_Update]
 ON [dbo].[MaritalRelationship] FOR Update
 AS 
 IF ( 
 dbo.Equals( (SELECT TOP 1 [RelationshipId] FROM INSERTED) , (SELECT TOP 1 [RelationshipId] FROM DELETED)) = 0 OR 
 dbo.Equals( (SELECT TOP 1 [MaritalStatusId] FROM INSERTED) , (SELECT TOP 1 [MaritalStatusId] FROM DELETED)) = 0 
 ) 
 INSERT INTO [MaritalRelationshipHistory]
 (OperationTypeId, ChangeDate, RelationshipId, MaritalStatusId)
 SELECT 
2, getdate(),RelationshipId,MaritalStatusId
 FROM INSERTED


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TimedRelationshipHistory](
	[TimedRelationshipHistoryId] [uniqueidentifier] NOT NULL CONSTRAINT [DF_TimedRelationshipHistory_AddressHistoryId]  DEFAULT (newid()),
	[ChangeDate] [datetime] NOT NULL CONSTRAINT [DF_TimedRelationshipHistory_ChangeDate]  DEFAULT (getdate()),
	[OperationTypeId] [int] NOT NULL,
	[RelationshipId] [uniqueidentifier] NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
 CONSTRAINT [PK_TimedRelationshipHistory] PRIMARY KEY CLUSTERED 
(
	[TimedRelationshipHistoryId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RelationshipHistory](
	[RelationshipHistoryId] [uniqueidentifier] NOT NULL CONSTRAINT [DF_RelationshipHistory_AddressHistoryId]  DEFAULT (newid()),
	[ChangeDate] [datetime] NOT NULL CONSTRAINT [DF_RelationshipHistory_ChangeDate]  DEFAULT (getdate()),
	[OperationTypeId] [int] NOT NULL,
	[RelationshipId] [uniqueidentifier] NULL,
	[PersonId] [uniqueidentifier] NULL,
	[RelatedPersonId] [uniqueidentifier] NULL,
	[RelationshipTypeId] [int] NULL,
	[RegistrationDate] [datetime] NULL,
 CONSTRAINT [PK_RelationshipHistory] PRIMARY KEY CLUSTERED 
(
	[RelationshipHistoryId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PersonHistory](
	[PersonHistoryId] [uniqueidentifier] NOT NULL CONSTRAINT [DF_PersonHistory_PersonHistoryId]  DEFAULT (newid()),
	[ChangeDate] [datetime] NOT NULL CONSTRAINT [DF_PersonHistory_ChangeDate]  DEFAULT (getdate()),
	[OperationTypeId] [int] NOT NULL,
	[PersonId] [uniqueidentifier] NOT NULL,
	[NationalityCountryAlpha2Code] [varchar](2) NULL,
	[PersonNumber] [char](10) NULL,
	[FirstName] [nvarchar](34) NULL,
	[MiddleName] [nvarchar](34) NULL,
	[LastName] [nvarchar](34) NULL,
	[NameForAddressing] [nvarchar](34) NULL,
	[BirthDate] [datetime] NULL,
	[BirthDateUncertaintyIndicator] [bit] NULL,
	[DeathDate] [datetime] NULL,
	[DeathDateUncertaintyIndicator] [bit] NULL,
	[MaritalStatusTypeId] [int] NULL,
	[PersonStatusTypeId] [int] NULL,
	[PersonStatusDate] [datetime] NULL,
	[GenderId] [int] NULL,
	[PersonProtectionIndicator] [bit] NULL,
	[PersonProtectionIndicatorStartDate] [datetime] NULL,
	[PersonProtectionIndicatorEndDate] [datetime] NULL,
	[IsTestPerson] [bit] NULL,
 CONSTRAINT [PK_PersonHistory] PRIMARY KEY CLUSTERED 
(
	[PersonHistoryId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE NONCLUSTERED INDEX [IX_PersonHistory_ChangeDate] ON [dbo].[PersonHistory] 
(
	[ChangeDate] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_PersonHistory_PersonId] ON [dbo].[PersonHistory] 
(
	[PersonId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AddressHistory](
	[AddressHistoryId] [uniqueidentifier] NOT NULL CONSTRAINT [DF_AddressHistory_AddressHistoryId]  DEFAULT (newid()),
	[ChangeDate] [datetime] NOT NULL CONSTRAINT [DF_AddressHistory_ChangeDate]  DEFAULT (getdate()),
	[OperationTypeId] [int] NOT NULL,
	[PersonId] [uniqueidentifier] NULL,
	[AddressStatusId] [int] NULL,
	[CountryAlpha2Code] [varchar](2) NULL,
	[MunicipalityCode] [varchar](4) NULL,
	[StreetCode] [varchar](4) NULL,
	[StreetName] [nvarchar](50) NULL,
	[StreetNameForAddressing] [nvarchar](50) NULL,
	[StreetBuildingIdentifier] [varchar](4) NULL,
	[FloorIdentifier] [varchar](2) NULL,
	[SuiteIdentifier] [nvarchar](4) NULL,
	[MailDeliverSubLocationIdentifier] [nvarchar](34) NULL,
	[DistrictSubDivisionIdentifier] [nvarchar](34) NULL,
	[PostBoxIdentifier] [nvarchar](4) NULL,
	[PostCode] [varchar](10) NULL,
	[PostDistrictName] [nvarchar](20) NULL,
	[CareOfName] [nvarchar](50) NULL,
	[Line1] [nvarchar](50) NULL,
	[Line2] [nvarchar](50) NULL,
	[Line3] [nvarchar](50) NULL,
	[Line4] [nvarchar](50) NULL,
	[Line5] [nvarchar](50) NULL,
	[Line6] [nvarchar](50) NULL,
	[LocationDescription] [nvarchar](50) NULL,
 CONSTRAINT [PK_AddressHistory] PRIMARY KEY CLUSTERED 
(
	[AddressHistoryId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MaritalRelationshipHistory](
	[MaritalRelationshipHistoryId] [uniqueidentifier] NOT NULL CONSTRAINT [DF_MaritalRelationshipHistory_AddressHistoryId]  DEFAULT (newid()),
	[ChangeDate] [datetime] NOT NULL CONSTRAINT [DF_MaritalRelationshipHistory_ChangeDate]  DEFAULT (getdate()),
	[OperationTypeId] [int] NOT NULL,
	[RelationshipId] [uniqueidentifier] NULL,
	[MaritalStatusId] [int] NULL,
 CONSTRAINT [PK_MaritalRelationshipHistory] PRIMARY KEY CLUSTERED 
(
	[MaritalRelationshipHistoryId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BirthdateNotificationPerson](
	[NotificationPersonId] [uniqueidentifier] NOT NULL,
	[Age] [int] NOT NULL,
 CONSTRAINT [PK_BirthdateNotificationPerson] PRIMARY KEY CLUSTERED 
(
	[NotificationPersonId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustodyRelationship](
	[RelationshipId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_CustodyRelationship] PRIMARY KEY CLUSTERED 
(
	[RelationshipId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
 CONSTRAINT [PK_LogEntry] PRIMARY KEY CLUSTERED 
(
	[LogEntryId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DataProvider](
	[DataProviderId] [int] IDENTITY(1,1) NOT NULL,
	[DataProviderTypeId] [int] NOT NULL,
	[Address] [varchar](50) NOT NULL,
	[Port] [int] NULL,
	[ConnectionString] [varchar](250) NULL,
	[UserName] [varchar](50) NULL,
	[Password] [varchar](50) NULL,
 CONSTRAINT [PK_DataProvider] PRIMARY KEY CLUSTERED 
(
	[DataProviderId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[GetDueNotifications]
(
		@Now datetime,
		@LastTime datetime
)

AS
	-- Select all the subscriptions that should be fired
	SELECT S.*
	FROM Application AS A 
	INNER JOIN Subscription AS S ON S.ApplicationId = A.ApplicationId
	INNER JOIN Channel AS C ON C.SubscriptionId = S.SubscriptionId
	LEFT OUTER JOIN BirthdateSubscription AS BDS ON BDS.SubscriptionId = S.SubscriptionId	
	LEFT OUTER JOIN DataSubscription AS DS ON DS.SubscriptionId = S.SubscriptionId	
	WHERE 
	  A.IsApproved = 1
	AND 
	  EXISTS (
		SELECT P.PersonId
		FROM Person AS P
		WHERE 
		-- Person match
		(
			S.IsForAllPersons = 1 
			OR EXISTS (
				SELECT PS.SubscriptionPersonId 
				FROM SubscriptionPerson AS PS
				WHERE PS.SubscriptionId = S.SubscriptionId AND PS.PersonId = P.PersonId
				)
		)
		
		AND 
		(
			-- Match birthdate
			(
				BDS.SubscriptionId IS NOT NULL 
				AND dbo.IsBirthdateEvent(@Now, P.BirthDate, BDS.AgeYears, BDS.PriorDays) = 1		
			)
			OR			
			-- Match data change
			(
				DS.SubscriptionId IS NOT NULL
				AND 
				(
					dbo.IsDataChangeEvent(P.PersonId, @Now, @LastTime) = 1
				)
			)
		)
		
		
	)
	


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[InsertBirthdateNotificationData]
(
	@SubscriptionId uniqueidentifier,
	@Today DateTime
)

AS
	DECLARE @NotificationId UNIQUEIDENTIFIER
	
	DECLARE @AgeYears INT
	DECLARE @PriorDays INT
	DECLARE @IsForAllPersons BIT
	
	-- Get subscription parameters
	SELECT @AgeYears = AgeYears, @PriorDays=PriorDays, @IsForAllPersons= IsForAllPersons
	FROM Subscription AS S 
	INNER JOIN BirthdateSubscription AS BDS ON S.SubscriptionId = BDS.SubscriptionId
	WHERE S.SubscriptionId = @SubscriptionId
	
	-- Temp table to hold persons
	CREATE TABLE #Person (NotificationPersonID UNIQUEIDENTIFIER DEFAULT NEWID(), PersonID UNIQUEIDENTIFIER, Birthdate DATETIME, Age INT)
	
	-- Search  for persons that match the subscription rule
	INSERT INTO #Person (PersonId, Birthdate, Age)
	SELECT P.PersonID, P.BirthDate, DATEDIFF(YEAR, P.Birthdate, DATEADD(day, @PriorDays, @Today))
	FROM Person AS P
	LEFT OUTER JOIN SubscriptionPerson AS SP ON P.PersonId = SP.PersonId
	WHERE 
	(
		@IsForAllPersons = 1 OR SP.SubscriptionId = @SubscriptionId
	)
	AND
	(
		dbo.IsBirthdateEvent(@Today, P.BirthDate, @AgeYears, @PriorDays) = 1
	)
	
	IF EXISTS (SELECT * FROM #Person)-- If persons are found
	BEGIN
		-- Insert Notification row and child rows
		SET @NotificationID = NEWID()
		
		INSERT INTO Notification (NotificationId, SubscriptionId, NotificationDate)
		VALUES (@NotificationId, @SubscriptionId, @Today)
		
		INSERT INTO BirthdateNotification (NotificationID, AgeYears, PriorDays)
		VALUES (@NotificationId, @AgeYears, @PriorDays)
		
		INSERT INTO NotificationPerson (NotificationPersonId, NotificationId, PersonId)
		SELECT NotificationPersonId, @NotificationId, PersonId
		FROM #Person
		
		INSERT INTO BirthdateNotificationPerson(NotificationPersonID, Age)
		SELECT P.NotificationPersonID, P.Age
		FROM #Person P
		INNER JOIN NotificationPerson NP ON P.NotificationPersonID = NP.NotificationPersonID
	END
	
	-- Finally, select the new Notification row
	SELECT * 
	FROM Notification	
	WHERE NotificationId = @NotificationId


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[InsertChangeNotificationData]
(
	@SubscriptionId UNIQUEIDENTIFIER,
	@Today DATETIME,
	@LastTime DATETIME
)

AS
	DECLARE @NotificationId UNIQUEIDENTIFIER
	DECLARE @IsForAllPersons BIT
	
	-- Select subscription data
	SELECT @IsForAllPersons= IsForAllPersons
	FROM Subscription AS S 
	INNER JOIN DataSubscription AS DS ON S.SubscriptionId = DS.SubscriptionId
	WHERE S.SubscriptionId = @SubscriptionId
	
	-- Use this table to store temporary data
	CREATE TABLE #Person (NotificationPersonId UNIQUEIDENTIFIER DEFAULT NEWID(), PersonId UNIQUEIDENTIFIER)
	
	-- Search for persons that could possible fire the notification
	INSERT INTO #Person (PersonId)
	SELECT P.PersonId
	FROM Person AS P
	LEFT OUTER JOIN SubscriptionPerson AS SP ON P.PersonId = SP.PersonId
	WHERE 
	(
		@IsForAllPersons = 1 OR SP.SubscriptionId = @SubscriptionId
	)
	AND
	(
		dbo.IsDataChangeEvent(P.PersonId, @Today, @LastTime) = 1
	)
	
	
	IF EXISTS (SELECT TOP 1 * FROM #Person) -- If persons are found
	BEGIN
		-- Insert data into physical tables
		SET @NotificationID = NEWID()
		
		INSERT INTO Notification (NotificationId, SubscriptionId, NotificationDate)
		VALUES (@NotificationId, @SubscriptionId, @Today)
		
		INSERT INTO NotificationPerson (NotificationId, NotificationPersonId, PersonId)
		SELECT @NotificationId, NotificationPersonId, PersonId
		FROM #Person		
		
	END
	
	-- Finally, return the new Notification object
	SELECT * 
	FROM Notification 
	WHERE NotificationId = @NotificationId

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[IsDataChangeEvent]
(
	@PersonId UNIQUEIDENTIFIER,
	@Now DATETIME,
	@LastTime DATETIME
)
RETURNS BIT
AS
	BEGIN
		DECLARE @RET BIT
		SET @RET = 0
		
		IF EXISTS
			( 
				-- If data has changed in Person table
				SELECT PersonHistoryId 
				FROM PersonHistory AS PH
				WHERE 
					PH.PersonId = @PersonId
				AND	PH.ChangeDate < @Now
				AND PH.ChangeDate >= @LastTime 
			)
		OR 
			EXISTS
			( 
				-- If data has changed in Address table
				SELECT AddressHistoryId 
				FROM AddressHistory AS AH
				WHERE 
					AH.PersonId = @PersonId
				AND	AH.ChangeDate < @Now
				AND AH.ChangeDate >= @LastTime 
			)
		OR
			EXISTS
			(
				-- If data has changed in Relationship table
				SELECT RelationshipHistoryId 
				FROM RelationshipHistory AS RH
				WHERE 
					RH.PersonId = @PersonId
				AND	RH.ChangeDate < @Now
				AND RH.ChangeDate >= @LastTime 
			)
		OR 
			EXISTS
			(
				-- if data has changed in TimedRelationship table
				SELECT TimedRelationshipHistoryId 
				FROM TimedRelationshipHistory AS TRH
				INNER JOIN RelationshipHistory AS RH ON TRH.RelationshipId = RH.RelationshipId
				WHERE 
					RH.PersonId = @PersonId
				AND	TRH.ChangeDate < @Now
				AND TRH.ChangeDate >= @LastTime 
			)
		OR
			EXISTS
			(
				-- if data has changed in MaritalRelationship table
				SELECT MaritalRelationshipHistoryId 
				FROM MaritalRelationshipHistory AS MRH
				INNER JOIN RelationshipHistory AS RH ON MRH.RelationshipId = RH.RelationshipId
				WHERE 
					RH.PersonId = @PersonId
				AND	MRH.ChangeDate < @Now
				AND MRH.ChangeDate >= @LastTime 
			)
			SET @RET = 1
		RETURN @RET
	END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[CreateHistoryTriggers]
	
AS

	DECLARE @HistoryPrefix VARCHAR(50);
	SET @HistoryPrefix = 'History'
	
	DECLARE @SQL VARCHAR(MAX)
	SET @SQL = ''
	DECLARE @Tables TABLE (
		[Name] VARCHAR(250)
		)
	INSERT INTO @Tables 
	SELECT T.Name
	FROM sys.Tables AS T 
	INNER JOIN sys.Tables AS HT ON HT.Name = T.Name + @HistoryPrefix
	
	DECLARE @TableName VARCHAR(250)
	DECLARE TableCursor CURSOR FOR SELECT [Name] FROM @Tables
	
	-- Loop over all tables
	OPEN TableCursor 
	FETCH NEXT FROM TableCursor INTO @TableName
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
	
		DECLARE @OperationTypeId INT
		DECLARE @OperationName VARCHAR(50)
		DECLARE @SourceTableName VARCHAR(50)
		DECLARE @OldTableName VARCHAR(50)
		
		DECLARE @HistoryTableName VARCHAR(250)
		SET @HistoryTableName = @TableName + @HistoryPrefix 
		
		DECLARE @Columns TABLE ([Name] VARCHAR(250) )
		
		-- Get a list of columns that are in both the main table and the history table
		INSERT INTO 
			@Columns ([Name])
		SELECT 
			C.Name 
		FROM sys.Tables AS T,
			sys.Columns AS C,
			sys.Tables AS HT,
			sys.Columns AS HC		
		WHERE 
				T.Name = @TableName
			AND C.object_id = T.object_id
			AND HT.Name = @HistoryTableName
			AND HC.object_id = HT.object_id
			AND HC.Name = C.Name
			
		
		-- Now create a cursor to loop over operations from OperationType	
		DECLARE OperationCursor CURSOR FOR 
		SELECT OperationTypeId, OperationName, SourceTableName, OldTableName
		FROM OperationType		
		
		OPEN OperationCursor
		FETCH NEXT FROM OperationCursor INTO @OperationTypeId, @OperationName, @SourceTableName, @OldTableName
		-- Loop over all operations 
		WHILE @@FETCH_STATUS = 0
		BEGIN
			-- Set trigger name
			DECLARE @TriggerName VARCHAR(250)
			SET @TriggerName = @TableName + '_' + @OperationName ;
			
			-- Drop existing trigger (if any)
			SET @SQL = '' 
			+ 'IF EXISTS (SELECT * FROM sys.Triggers WHERE Name = ''' + @TriggerName +''')' + CHAR(10)
			+ ' DROP TRIGGER ' + @TriggerName + CHAR(10)
			
			EXEC (@SQL)
			
			-- Create new trigger
			SET @SQL = '' 			
			+ 'CREATE TRIGGER [' + @TriggerName + ']' + CHAR(10)
			+ ' ON [' + @TableName + ']'
			+ ' FOR ' + @OperationName + CHAR(10)
			+ ' AS ' + CHAR(10)
			
			-- If there are two tables (incase of update), only insert if at least one column has a new value
			IF @OldTableName IS NOT NULL
			BEGIN
				SET @SQL = @SQL + ' IF ( ' + CHAR(10)
				SELECT @SQL = @SQL + ' dbo.Equals( '
				+'(SELECT TOP 1 ['+ Name + '] FROM '+ @SourceTableName +') , (SELECT TOP 1 ['+ Name + '] FROM '+ @OldTableName + ')) = 0 OR ' + CHAR(10)
				FROM @Columns
				SET @SQL = CHAR(10) + SUBSTRING(@SQL, 1,  LEN(@SQL) - 4 ) + CHAR(10) + ' ) ' + CHAR(10)
			END
			
			-- Insert operation related columns
			SET @SQL = @SQL
			+ ' INSERT INTO [' + @HistoryTableName + ']' + CHAR(10)
			+ ' (OperationTypeId, ChangeDate'
			
			-- Insert data columns
			SELECT @SQL = @SQL + ', ' + [Name]
			FROM @Columns
					
			-- Operation column values
			SET @SQL = @SQL + ')' + CHAR(10)
			+ ' SELECT ' + CHAR(10)
			+ CAST (@OperationTypeId AS VARCHAR(1) ) + ', '
			+ 'getdate()'
			
			-- Data column values
			SELECT @SQL = @SQL + ',' + [Name]
			FROM @Columns
			
			SET @SQL = @SQL  + CHAR(10)
			+ ' FROM ' + @SourceTableName + CHAR(10) 
			+ CHAR(10)
			
			PRINT @SQL
			EXEC (@SQL)
			
			FETCH NEXT FROM OperationCursor INTO @OperationTypeId, @OperationName, @SourceTableName, @OldTableName
		END
		
		CLOSE OperationCursor
		DEALLOCATE OperationCursor
		
		DELETE @Columns
		FETCH NEXT FROM TableCursor	INTO @TableName
	END
	CLOSE TableCursor
	DEALLOCATE TableCursor
	

GO
ALTER TABLE [dbo].[Person]  WITH CHECK ADD  CONSTRAINT [FK_Person_Country] FOREIGN KEY([NationalityCountryAlpha2Code])
REFERENCES [dbo].[Country] ([Alpha2Code])
GO
ALTER TABLE [dbo].[Person] CHECK CONSTRAINT [FK_Person_Country]
GO
ALTER TABLE [dbo].[Person]  WITH CHECK ADD  CONSTRAINT [FK_Person_DetailLevel] FOREIGN KEY([DetailLevelId])
REFERENCES [dbo].[DetailLevel] ([DetailLevelId])
GO
ALTER TABLE [dbo].[Person] CHECK CONSTRAINT [FK_Person_DetailLevel]
GO
ALTER TABLE [dbo].[Person]  WITH CHECK ADD  CONSTRAINT [FK_Person_Gender] FOREIGN KEY([GenderId])
REFERENCES [dbo].[Gender] ([GenderId])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[Person] CHECK CONSTRAINT [FK_Person_Gender]
GO
ALTER TABLE [dbo].[Person]  WITH CHECK ADD  CONSTRAINT [FK_Person_MaritalStatusType] FOREIGN KEY([MaritalStatusTypeId])
REFERENCES [dbo].[MaritalStatusType] ([MaritalStatusTypeId])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[Person] CHECK CONSTRAINT [FK_Person_MaritalStatusType]
GO
ALTER TABLE [dbo].[Person]  WITH CHECK ADD  CONSTRAINT [FK_Person_PersonStatusType] FOREIGN KEY([PersonStatusTypeCode])
REFERENCES [dbo].[PersonStatusType] ([PersonStatusTypeCode])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[Person] CHECK CONSTRAINT [FK_Person_PersonStatusType]
GO
ALTER TABLE [dbo].[Address]  WITH CHECK ADD  CONSTRAINT [FK_Address_AddressStatus] FOREIGN KEY([AddressStatusId])
REFERENCES [dbo].[AddressStatus] ([AddressStatusId])
GO
ALTER TABLE [dbo].[Address] CHECK CONSTRAINT [FK_Address_AddressStatus]
GO
ALTER TABLE [dbo].[Address]  WITH CHECK ADD  CONSTRAINT [FK_Address_Country] FOREIGN KEY([CountryAlpha2Code])
REFERENCES [dbo].[Country] ([Alpha2Code])
GO
ALTER TABLE [dbo].[Address] CHECK CONSTRAINT [FK_Address_Country]
GO
ALTER TABLE [dbo].[Address]  WITH CHECK ADD  CONSTRAINT [FK_Address_Municipality] FOREIGN KEY([MunicipalityCode])
REFERENCES [dbo].[Municipality] ([MunicipalityCode])
GO
ALTER TABLE [dbo].[Address] CHECK CONSTRAINT [FK_Address_Municipality]
GO
ALTER TABLE [dbo].[Address]  WITH CHECK ADD  CONSTRAINT [FK_Address_Person] FOREIGN KEY([PersonId])
REFERENCES [dbo].[Person] ([PersonId])
GO
ALTER TABLE [dbo].[Address] CHECK CONSTRAINT [FK_Address_Person]
GO
ALTER TABLE [dbo].[Relationship]  WITH CHECK ADD  CONSTRAINT [FK_PersonRelationship_Person] FOREIGN KEY([PersonId])
REFERENCES [dbo].[Person] ([PersonId])
GO
ALTER TABLE [dbo].[Relationship] CHECK CONSTRAINT [FK_PersonRelationship_Person]
GO
ALTER TABLE [dbo].[Relationship]  WITH CHECK ADD  CONSTRAINT [FK_PersonRelationship_RelatedPerson] FOREIGN KEY([RelatedPersonId])
REFERENCES [dbo].[Person] ([PersonId])
GO
ALTER TABLE [dbo].[Relationship] CHECK CONSTRAINT [FK_PersonRelationship_RelatedPerson]
GO
ALTER TABLE [dbo].[Relationship]  WITH CHECK ADD  CONSTRAINT [FK_Relationship_RelationshipType] FOREIGN KEY([RelationshipTypeId])
REFERENCES [dbo].[RelationshipType] ([RelationshipTypeId])
GO
ALTER TABLE [dbo].[Relationship] CHECK CONSTRAINT [FK_Relationship_RelationshipType]
GO
ALTER TABLE [dbo].[SubscriptionPerson]  WITH CHECK ADD  CONSTRAINT [FK_SubscriptionPerson_Person] FOREIGN KEY([PersonId])
REFERENCES [dbo].[Person] ([PersonId])
GO
ALTER TABLE [dbo].[SubscriptionPerson] CHECK CONSTRAINT [FK_SubscriptionPerson_Person]
GO
ALTER TABLE [dbo].[SubscriptionPerson]  WITH CHECK ADD  CONSTRAINT [FK_SubscriptionPerson_Subscription] FOREIGN KEY([SubscriptionId])
REFERENCES [dbo].[Subscription] ([SubscriptionId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SubscriptionPerson] CHECK CONSTRAINT [FK_SubscriptionPerson_Subscription]
GO
ALTER TABLE [dbo].[NotificationPerson]  WITH CHECK ADD  CONSTRAINT [FK_NotificationPerson_Notification] FOREIGN KEY([NotificationId])
REFERENCES [dbo].[Notification] ([NotificationId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[NotificationPerson] CHECK CONSTRAINT [FK_NotificationPerson_Notification]
GO
ALTER TABLE [dbo].[NotificationPerson]  WITH CHECK ADD  CONSTRAINT [FK_NotificationPerson_Person] FOREIGN KEY([PersonId])
REFERENCES [dbo].[Person] ([PersonId])
GO
ALTER TABLE [dbo].[NotificationPerson] CHECK CONSTRAINT [FK_NotificationPerson_Person]
GO
ALTER TABLE [dbo].[Notification]  WITH CHECK ADD  CONSTRAINT [FK_Notification_Subscription] FOREIGN KEY([SubscriptionId])
REFERENCES [dbo].[Subscription] ([SubscriptionId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Notification] CHECK CONSTRAINT [FK_Notification_Subscription]
GO
ALTER TABLE [dbo].[Channel]  WITH CHECK ADD  CONSTRAINT [FK_Channel_ChannelType] FOREIGN KEY([ChannelTypeId])
REFERENCES [dbo].[ChannelType] ([ChannelTypeId])
GO
ALTER TABLE [dbo].[Channel] CHECK CONSTRAINT [FK_Channel_ChannelType]
GO
ALTER TABLE [dbo].[Channel]  WITH CHECK ADD  CONSTRAINT [FK_Channel_Subscription] FOREIGN KEY([SubscriptionId])
REFERENCES [dbo].[Subscription] ([SubscriptionId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Channel] CHECK CONSTRAINT [FK_Channel_Subscription]
GO
ALTER TABLE [dbo].[BirthdateSubscription]  WITH CHECK ADD  CONSTRAINT [FK_BirthdateSubscription_Subscription] FOREIGN KEY([SubscriptionId])
REFERENCES [dbo].[Subscription] ([SubscriptionId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BirthdateSubscription] CHECK CONSTRAINT [FK_BirthdateSubscription_Subscription]
GO
ALTER TABLE [dbo].[DataSubscription]  WITH CHECK ADD  CONSTRAINT [FK_DataSubscription_Subscription] FOREIGN KEY([SubscriptionId])
REFERENCES [dbo].[Subscription] ([SubscriptionId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DataSubscription] CHECK CONSTRAINT [FK_DataSubscription_Subscription]
GO
ALTER TABLE [dbo].[GpacChannel]  WITH CHECK ADD  CONSTRAINT [FK_GpacChannel_Channel1] FOREIGN KEY([ChannelId])
REFERENCES [dbo].[Channel] ([ChannelId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[GpacChannel] CHECK CONSTRAINT [FK_GpacChannel_Channel1]
GO
ALTER TABLE [dbo].[ChildRelationship]  WITH CHECK ADD  CONSTRAINT [FK_ChildRelationship_Relationship] FOREIGN KEY([RelationshipId])
REFERENCES [dbo].[Relationship] ([RelationshipId])
GO
ALTER TABLE [dbo].[ChildRelationship] CHECK CONSTRAINT [FK_ChildRelationship_Relationship]
GO
ALTER TABLE [dbo].[TimedRelationship]  WITH CHECK ADD  CONSTRAINT [FK_TimedRelationship_Relationship] FOREIGN KEY([RelationshipId])
REFERENCES [dbo].[Relationship] ([RelationshipId])
GO
ALTER TABLE [dbo].[TimedRelationship] CHECK CONSTRAINT [FK_TimedRelationship_Relationship]
GO
ALTER TABLE [dbo].[BirthdateNotification]  WITH CHECK ADD  CONSTRAINT [FK_BirthdateNotification_Notification] FOREIGN KEY([NotificationId])
REFERENCES [dbo].[Notification] ([NotificationId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BirthdateNotification] CHECK CONSTRAINT [FK_BirthdateNotification_Notification]
GO
ALTER TABLE [dbo].[Subscription]  WITH CHECK ADD  CONSTRAINT [FK_Subscription_Application] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[Application] ([ApplicationId])
GO
ALTER TABLE [dbo].[Subscription] CHECK CONSTRAINT [FK_Subscription_Application]
GO
ALTER TABLE [dbo].[Subscription]  WITH CHECK ADD  CONSTRAINT [FK_Subscription_SubscriptionType] FOREIGN KEY([SubscriptionTypeId])
REFERENCES [dbo].[SubscriptionType] ([SubscriptionTypeId])
GO
ALTER TABLE [dbo].[Subscription] CHECK CONSTRAINT [FK_Subscription_SubscriptionType]
GO
ALTER TABLE [dbo].[MaritalRelationship]  WITH CHECK ADD  CONSTRAINT [FK_MaritalRelationship_TimedRelationship] FOREIGN KEY([RelationshipId])
REFERENCES [dbo].[TimedRelationship] ([RelationshipId])
GO
ALTER TABLE [dbo].[MaritalRelationship] CHECK CONSTRAINT [FK_MaritalRelationship_TimedRelationship]
GO
ALTER TABLE [dbo].[MaritalRelationship]  WITH CHECK ADD  CONSTRAINT [FK_PersonMaritalStatus_MaritalStatusType] FOREIGN KEY([MaritalStatusId])
REFERENCES [dbo].[MaritalStatusType] ([MaritalStatusTypeId])
GO
ALTER TABLE [dbo].[MaritalRelationship] CHECK CONSTRAINT [FK_PersonMaritalStatus_MaritalStatusType]
GO
ALTER TABLE [dbo].[RelationshipHistory]  WITH CHECK ADD  CONSTRAINT [FK_RelationshipHistory_OperationType] FOREIGN KEY([OperationTypeId])
REFERENCES [dbo].[OperationType] ([OperationTypeId])
GO
ALTER TABLE [dbo].[RelationshipHistory] CHECK CONSTRAINT [FK_RelationshipHistory_OperationType]
GO
ALTER TABLE [dbo].[TimedRelationshipHistory]  WITH CHECK ADD  CONSTRAINT [FK_TimedRelationshipHistory_OperationType] FOREIGN KEY([OperationTypeId])
REFERENCES [dbo].[OperationType] ([OperationTypeId])
GO
ALTER TABLE [dbo].[TimedRelationshipHistory] CHECK CONSTRAINT [FK_TimedRelationshipHistory_OperationType]
GO
ALTER TABLE [dbo].[MaritalRelationshipHistory]  WITH CHECK ADD  CONSTRAINT [FK_MaritalRelationshipHistory_OperationType] FOREIGN KEY([OperationTypeId])
REFERENCES [dbo].[OperationType] ([OperationTypeId])
GO
ALTER TABLE [dbo].[MaritalRelationshipHistory] CHECK CONSTRAINT [FK_MaritalRelationshipHistory_OperationType]
GO
ALTER TABLE [dbo].[PersonHistory]  WITH CHECK ADD  CONSTRAINT [FK_PersonHistory_OperationType] FOREIGN KEY([OperationTypeId])
REFERENCES [dbo].[OperationType] ([OperationTypeId])
GO
ALTER TABLE [dbo].[PersonHistory] CHECK CONSTRAINT [FK_PersonHistory_OperationType]
GO
ALTER TABLE [dbo].[AddressHistory]  WITH CHECK ADD  CONSTRAINT [FK_AddressHistory_OperationType] FOREIGN KEY([OperationTypeId])
REFERENCES [dbo].[OperationType] ([OperationTypeId])
GO
ALTER TABLE [dbo].[AddressHistory] CHECK CONSTRAINT [FK_AddressHistory_OperationType]
GO
ALTER TABLE [dbo].[BirthdateNotificationPerson]  WITH CHECK ADD  CONSTRAINT [FK_BirthdateNotificationPerson_NotificationPerson] FOREIGN KEY([NotificationPersonId])
REFERENCES [dbo].[NotificationPerson] ([NotificationPersonId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BirthdateNotificationPerson] CHECK CONSTRAINT [FK_BirthdateNotificationPerson_NotificationPerson]
GO
ALTER TABLE [dbo].[CustodyRelationship]  WITH CHECK ADD  CONSTRAINT [FK_CustodyRelationship_TimedRelationship] FOREIGN KEY([RelationshipId])
REFERENCES [dbo].[TimedRelationship] ([RelationshipId])
GO
ALTER TABLE [dbo].[CustodyRelationship] CHECK CONSTRAINT [FK_CustodyRelationship_TimedRelationship]
GO
ALTER TABLE [dbo].[LogEntry]  WITH NOCHECK ADD  CONSTRAINT [FK_LogEntry_Application] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[Application] ([ApplicationId])
GO
ALTER TABLE [dbo].[LogEntry] NOCHECK CONSTRAINT [FK_LogEntry_Application]
GO
ALTER TABLE [dbo].[LogEntry]  WITH CHECK ADD  CONSTRAINT [FK_LogEntry_LogType] FOREIGN KEY([LogTypeId])
REFERENCES [dbo].[LogType] ([LogTypeId])
GO
ALTER TABLE [dbo].[LogEntry] CHECK CONSTRAINT [FK_LogEntry_LogType]
GO
ALTER TABLE [dbo].[DataProvider]  WITH CHECK ADD  CONSTRAINT [FK_DataProvider_DataProviderType] FOREIGN KEY([DataProviderTypeId])
REFERENCES [dbo].[DataProviderType] ([DataProviderTypeId])
GO
ALTER TABLE [dbo].[DataProvider] CHECK CONSTRAINT [FK_DataProvider_DataProviderType]
