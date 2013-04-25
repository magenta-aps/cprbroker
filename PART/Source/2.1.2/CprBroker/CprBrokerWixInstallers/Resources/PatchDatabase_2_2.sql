-----------------------------------------------------------------
------------  Enables parameterized subscriptions  -------------
-----------------------------------------------------------------

IF NOT EXISTS (SELECT * FROM sys.columns c WHERE name = 'PersonRegistrationId' and object_id=object_id('DataChangeEvent'))
	ALTER TABLE dbo.DataChangeEvent ADD PersonRegistrationId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID()
GO