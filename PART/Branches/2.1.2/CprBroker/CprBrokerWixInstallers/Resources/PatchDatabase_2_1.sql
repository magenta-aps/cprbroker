-------------------------------------
-----  Column width expansion  ------
-------------------------------------
ALTER TABLE [dbo].[PersonProperties] ALTER COLUMN [BirthPlace] varchar(132) 
GO


---------------------------------------
-----  Table for extract errors  ------
---------------------------------------

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='ExtractError')
CREATE TABLE dbo.ExtractError(
    ExtractErrorId uniqueidentifier NOT NULL DEFAULT(newid()),
    ExtractId uniqueidentifier NOT NULL,
    Contents nvarchar(157) NOT NULL,

    CONSTRAINT PK_ExtractError PRIMARY KEY CLUSTERED 
    (	ExtractErrorId ASC
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON),

    CONSTRAINT	FK_ExtractError_Extract FOREIGN KEY
    (	ExtractId
    ) REFERENCES dbo.Extract(ExtractId) ON UPDATE  CASCADE ON DELETE CASCADE 	
)

GO


-----------------------------------------------------------------
-----  Allow multiple attributes in a single registration  ------
-----------------------------------------------------------------

IF NOT EXISTS (SELECT * FROM sys.columns c WHERE name = 'PersonAttributesId' and object_id=object_id('PersonAttributes'))
	EXEC sp_rename 'dbo.PersonAttributes.PersonRegistrationId', 'PersonAttributesId', 'COLUMN'
GO

IF NOT EXISTS (SELECT * FROM sys.columns c WHERE name = 'PersonRegistrationId' and object_id=object_id('PersonAttributes'))
	ALTER TABLE dbo.PersonAttributes ADD PersonRegistrationId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID()
GO

UPDATE PA
SET PersonRegistrationId = PersonAttributesId
FROM dbo.PersonAttributes PA 
INNER JOIN dbo.PersonRegistration PR ON PR.PersonRegistrationId = PA.PersonAttributesID AND PR.PersonRegistrationId <> PA.PersonRegistrationId

GO

IF EXISTS (SELECT * FROM sys.sysconstraints WHERE constid= object_id('FK_PersonAttributes_PersonRegistration'))
	ALTER Table dbo.PersonAttributes DROP CONSTRAINT FK_PersonAttributes_PersonRegistration
GO

ALTER TABLE dbo.PersonAttributes ADD CONSTRAINT FK_PersonAttributes_PersonRegistration FOREIGN KEY(PersonRegistrationId) REFERENCES PersonRegistration(PersonRegistrationId) 
ON UPDATE CASCADE 
ON DELETE CASCADE

GO

IF NOT EXISTS (SELECT * FROM sys.columns c WHERE name = 'PersonAttributesId' and object_id=object_id('CprData'))
	EXEC sp_rename 'dbo.CprData.PersonRegistrationId', 'PersonAttributesId', 'COLUMN'
GO

IF NOT EXISTS (SELECT * FROM sys.columns c WHERE name = 'PersonAttributesId' and object_id=object_id('ForeignCitizenData'))
	EXEC sp_rename 'dbo.ForeignCitizenData.PersonRegistrationId', 'PersonAttributesId', 'COLUMN'
GO

IF NOT EXISTS (SELECT * FROM sys.columns c WHERE name = 'PersonAttributesId' and object_id=object_id('ForeignCitizenCountry'))
	EXEC sp_rename 'dbo.ForeignCitizenCountry.PersonRegistrationId', 'PersonAttributesId', 'COLUMN'
GO

IF NOT EXISTS (SELECT * FROM sys.columns c WHERE name = 'PersonAttributesId' and object_id=object_id('UnknownCitizenData'))
	EXEC sp_rename 'dbo.UnknownCitizenData.PersonRegistrationId', 'PersonAttributesId', 'COLUMN'
GO

IF NOT EXISTS (SELECT * FROM sys.columns c WHERE name = 'PersonAttributesId' and object_id=object_id('HealthInformation'))
	EXEC sp_rename 'dbo.HealthInformation.PersonRegistrationId', 'PersonAttributesId', 'COLUMN'
GO

IF NOT EXISTS (SELECT * FROM sys.columns c WHERE name = 'PersonAttributesId' and object_id=object_id('PersonProperties'))
	EXEC sp_rename 'dbo.PersonProperties.PersonRegistrationId', 'PersonAttributesId', 'COLUMN'
GO

IF NOT EXISTS (SELECT * FROM sys.columns c WHERE name = 'PersonAttributesId' and object_id=object_id('PersonName'))
	EXEC sp_rename 'dbo.PersonName.PersonRegistrationId', 'PersonAttributesId', 'COLUMN'
GO

IF EXISTS (SELECT * FROM sys.sysconstraints WHERE constid= object_id('FK_PersonProperties_Effect'))
	ALTER TABLE PersonProperties DROP CONSTRAINT FK_PersonProperties_Effect
GO

IF NOT EXISTS (SELECT * FROM sys.columns c WHERE name = 'EffectId' and object_id=object_id('PersonProperties'))
	ALTER TABLE PersonProperties DROP COLUMN EffectId
GO



-----------------------------------------------------
-----  UnknownCitizen_PersonAttributes cascade ------
-----------------------------------------------------

IF EXISTS (SELECT * FROM sys.sysconstraints WHERE constid= object_id('FK_UnknownCitizenData_PersonAttributes'))
	ALTER TABLE [dbo].[UnknownCitizenData]  DROP CONSTRAINT [FK_UnknownCitizenData_PersonAttributes] 
GO

ALTER TABLE [dbo].[UnknownCitizenData]  WITH CHECK ADD  CONSTRAINT [FK_UnknownCitizenData_PersonAttributes] FOREIGN KEY([PersonAttributesId])
REFERENCES [dbo].[PersonAttributes] ([PersonAttributesId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO