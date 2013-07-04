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
	ALTER TABLE dbo.Extract ADD ProcessedLines BIGINT NULL
GO

------------------------------
-- Table : ExtractPersonStaging
------------------------------

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ExtractPersonStaging_ExtractId' AND object_id=object_id('ExtractPersonStaging'))
	CREATE NONCLUSTERED INDEX [IX_ExtractPersonStaging_ExtractId] ON [dbo].[ExtractPersonStaging] 
	(
		[ExtractId] ASC
	)
	WITH (STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
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

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = object_id('PersonLifeState'))
	drop table dbo.PersonLifeState
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = object_id('PersonCivilState'))
	drop table dbo.PersonCivilState
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = object_id('PersonState'))
	drop table dbo.PersonState
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = object_id('PersonRelationship'))
	drop table dbo.PersonRelationship
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = object_id('UnknownCitizenData'))
	drop table dbo.UnknownCitizenData
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = object_id('PersonName'))
	drop table dbo.PersonName
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = object_id('PersonProperties'))
	drop table dbo.PersonProperties
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = object_id('HealthInformation'))
	drop table dbo.HealthInformation
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = object_id('ForeignCitizenCountry'))
	drop table dbo.ForeignCitizenCountry
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = object_id('ForeignCitizenData'))
	drop table dbo.ForeignCitizenData
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = object_id('CprData'))
	drop table dbo.CprData
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = object_id('PersonAttributes'))
	drop table dbo.PersonAttributes
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = object_id('ForeignAddress'))
	drop table dbo.ForeignAddress
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = object_id('GreenlandicAddress'))
	drop table dbo.GreenlandicAddress
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = object_id('GeographicCoorditaneTuple'))
	drop table dbo.GeographicCoorditaneTuple
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = object_id('GeographicPointLocation'))
	drop table dbo.GeographicPointLocation
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = object_id('AddressPointStatus'))
	drop table dbo.AddressPointStatus
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = object_id('AddressPoint'))
	drop table dbo.AddressPoint
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = object_id('DanishAddress'))
	drop table dbo.DanishAddress
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = object_id('DenmarkAddress'))
	drop table dbo.DenmarkAddress
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = object_id('Address'))
	drop table dbo.Address
GO