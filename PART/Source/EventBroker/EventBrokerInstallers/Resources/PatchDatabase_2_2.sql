-----------------------------------------------------------------
-----  Allow multiple attributes in a single registration  ------
-----------------------------------------------------------------

IF NOT EXISTS (SELECT * FROM sys.columns c WHERE name = 'PersonRegistrationId' and object_id=object_id('DataChangeEvent'))
	ALTER TABLE dbo.DataChangeEvent ADD PersonRegistrationId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID()
GO

IF NOT EXISTS (SELECT * FROM sys.columns c WHERE name = 'Criteria' and object_id=object_id('Subscription'))
	ALTER TABLE dbo.Subscription ADD Criteria XML NULL DEFAULT NULL
	ALTER TABLE dbo.Subscription ADD Created Datetime NOT NULL DEFAULT '1900-01-01 00:00:00'
	ALTER TABLE dbo.Subscription ADD Deactivated Datetime NULL DEFAULT NULL
GO

IF NOT EXISTS (SELECT * FROM sys.columns c WHERE name = 'Created' and object_id=object_id('SubscriptionPerson'))
	ALTER TABLE dbo.SubscriptionPerson ADD Created Datetime NOT NULL DEFAULT '1900-01-01 00:00:00'
	ALTER TABLE dbo.SubscriptionPerson ADD Removed Datetime NULL DEFAULT NULL
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id  = OBJECT_ID('BirthdateNotificationPerson'))
	DROP TABLE BirthdateNotificationPerson
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id  = OBJECT_ID('NotificationPerson'))
	DROP TABLE NotificationPerson
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id  = OBJECT_ID('BirthdateNotification'))
	DROP TABLE BirthdateNotification
GO



