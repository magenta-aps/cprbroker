----------------------------------------------------------------------------
------------  Fixes to error in installer of previous version  -------------
----------------------------------------------------------------------------

IF EXISTS (SELECT * FROM sys.columns c WHERE name = 'EffectId' and object_id=object_id('PersonProperties'))
	ALTER TABLE PersonProperties DROP COLUMN EffectId
GO

-----------------------------------------------------------------
------------  Enables parameterized subscriptions  -------------
-----------------------------------------------------------------

---------------------------
-- Table : DataChangeEvent
---------------------------

IF NOT EXISTS (SELECT * FROM sys.columns c WHERE name = 'PersonRegistrationId' and object_id=object_id('DataChangeEvent'))
	ALTER TABLE dbo.DataChangeEvent ADD PersonRegistrationId UNIQUEIDENTIFIER NOT NULL
GO

------------------------------
-- Table : PersonRegistration
------------------------------

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name= 'IX_PersonRegistration_UUID' AND object_id = object_id('PersonRegistration'))
	CREATE NONCLUSTERED INDEX [IX_PersonRegistration_UUID] ON [dbo].[PersonRegistration] ([UUID] ASC)
	WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO



------------------
-- Table : Extract
------------------

IF NOT EXISTS(SELECT * FROM sys.columns c WHERE name = 'ProcessedLines' and object_id=object_id('Extract'))
	ALTER TABLE dbo.Extrcat ADD ProcessedLines BIGINT NULL
GO

------------------------------
-- Table : ExtractPersonStaging
------------------------------

CREATE NONCLUSTERED INDEX [IX_ExtractPersonStaging_ExtractId] ON [dbo].[ExtractPersonStaging] 
(
	[ExtractId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


-----------------------
-- Table : ExtractItem
-----------------------

IF NOT EXISTS (SELECT * FROM sys.columns c WHERE name = 'RelationPNR2' and object_id=object_id('ExtractItem'))
	ALTER TABLE dbo.ExtractItem ADD RelationPNR2 VARCHAR(10) NULL
GO

----------------------------------------------------------
-- Getting rid of unused tables
----------------------------------------------------------

drop table dbo.PersonLifeState
drop table dbo.PersonCivilState
drop table dbo.PersonState
drop table dbo.PersonRelationship
drop table dbo.UnknownCitizenData
drop table dbo.PersonName
drop table dbo.PersonProperties
drop table dbo.HealthInformation
drop table dbo.ForeignCitizenCountry
drop table dbo.ForeignCitizenData
drop table dbo.CprData
drop table dbo.PersonAttributes
drop table dbo.ForeignAddress
drop table dbo.GreenlandicAddress
drop table dbo.GeographicCoorditaneTuple
drop table dbo.GeographicPointLocation
drop table dbo.AddressPointStatus
drop table dbo.AddressPoint
drop table dbo.DanishAddress
drop table dbo.DenmarkAddress
drop table dbo.Address
GO