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

