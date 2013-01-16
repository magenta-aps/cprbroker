-----------------------------------------------------------------
-----  Allow multiple attributes in a single registration  ------
-----------------------------------------------------------------

sp_rename 'dbo.PersonAttributes.PersonRegistrationId', 'PersonAttributesId', 'COLUMN'
GO
ALTER TABLE dbo.PersonAttributes ADD PersonRegistrationId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID()
GO
UPDATE dbo.PersonAttributes SET PersonRegistrationId = PersonAttributesId
GO
ALTER Table dbo.PersonAttributes DROP CONSTRAINT FK_PersonAttributes_PersonRegistration
GO
ALTER TABLE dbo.PersonAttributes ADD CONSTRAINT FK_PersonAttributes_PersonRegistration FOREIGN KEY(PersonRegistrationId) REFERENCES PersonRegistration(PersonRegistrationId) 
ON UPDATE CASCADE ON DELETE CASCADE
GO

sp_rename 'dbo.CprData.PersonRegistrationId', 'PersonAttributesId', 'COLUMN'
GO
sp_rename 'dbo.ForeignCitizenData.PersonRegistrationId', 'PersonAttributesId', 'COLUMN'
GO
sp_rename 'dbo.ForeignCitizenCountry.PersonRegistrationId', 'PersonAttributesId', 'COLUMN'
GO
sp_rename 'dbo.UnknownCitizenData.PersonRegistrationId', 'PersonAttributesId', 'COLUMN'
GO
sp_rename 'dbo.HealthInformation.PersonRegistrationId', 'PersonAttributesId', 'COLUMN'
GO
sp_rename 'dbo.PersonProperties.PersonRegistrationId', 'PersonAttributesId', 'COLUMN'
GO
sp_rename 'dbo.PersonName.PersonRegistrationId', 'PersonAttributesId', 'COLUMN'
GO
