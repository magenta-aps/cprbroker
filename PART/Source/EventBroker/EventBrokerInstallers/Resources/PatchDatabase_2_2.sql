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

IF NOT EXISTS (SELECT * FROM sys.columns c WHERE name = 'IsLastNotification' and object_id=object_id('EventNotification'))
	ALTER TABLE dbo.EventNotification ADD IsLastNotification bit NULL DEFAULT NULL
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

IF EXISTS (SELECT * FROM sys.tables WHERE object_id  = OBJECT_ID('Notification'))
	DROP TABLE [Notification]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EnqueueDataChangeEventNotifications]') AND type in (N'P', N'PC'))
	BEGIN
		DROP PROCEDURE dbo.EnqueueDataChangeEventNotifications
		EXEC dbo.sp_executesql @statement = N'
			CREATE Procedure [dbo].[EnqueueDataChangeEventNotifications]
			(
					--@StartDate DateTime,
					--@EndDate DateTime,
					@Now DateTime,
					@SubscriptionTypeId Int
			)


			AS
					-- Subscriptions (that ARE NOT deactivated) for specific persons that deactivated
					INSERT INTO EventNotification (SubscriptionId, PersonUUID, CreatedDate)
					SELECT S.SubscriptionId, DCE.PersonUuid, @Now
					FROM DataChangeEvent AS DCE
					INNER JOIN SubscriptionPerson AS SP ON SP.PersonUuid = DCE.PersonUuid
					INNER JOIN Subscription AS S ON S.SubscriptionId = SP.SubscriptionId
					WHERE 
						--DCE.ReceivedDate BETWEEN @StartDate AND @EndDate
						S.IsForAllPersons = 0
						AND S.SubscriptionTypeId = @SubscriptionTypeId
						-- We test if the subscription has been deactivated
						AND S.Deactivated IS NULL
		
					-- Subscriptions (that ARE deactivated) for specific persons
					INSERT INTO EventNotification (SubscriptionId, PersonUUID, CreatedDate, IsLastNotification)
					SELECT S.SubscriptionId, DCE.PersonUuid, @Now, 1
					FROM DataChangeEvent AS DCE
					INNER JOIN SubscriptionPerson AS SP ON SP.PersonUuid = DCE.PersonUuid
					INNER JOIN Subscription AS S ON S.SubscriptionId = SP.SubscriptionId
					WHERE 
						--DCE.ReceivedDate BETWEEN @StartDate AND @EndDate
						S.IsForAllPersons = 0
						AND S.SubscriptionTypeId = @SubscriptionTypeId
						-- We test if the subscription has been deactivated
						AND S.Deactivated IS NOT NULL
		
					-- Subscriptions for all persons
					INSERT INTO EventNotification (SubscriptionId, PersonUUID, CreatedDate)
					SELECT S.SubscriptionId, DCE.PersonUuid, @Now
					FROM DataChangeEvent AS DCE,	Subscription AS S	
					WHERE 
						--DCE.ReceivedDate BETWEEN @StartDate AND @EndDate
						S.IsForAllPersons = 1
						AND S.SubscriptionTypeId = @SubscriptionTypeId
			' 
	END
GO
