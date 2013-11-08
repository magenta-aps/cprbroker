----------------------------------------------------------------------------------------
---   Delete tables that are no longer used   --------------------------------------
----------------------------------------------------------------------------------------

IF EXISTS (SELECT * FROM sys.tables WHERE object_id  = OBJECT_ID('BirthdateNotificationPerson'))
	DROP TABLE BirthdateNotificationPerson
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = object_id('NotificationPerson'))
	DROP TABLE NotificationPerson
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = object_id('BirthdateNotification'))
	DROP TABLE BirthdateNotification
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = object_id('Notification'))
	DROP TABLE [Notification]
GO

----------------------------------------------------------------------------------------
---   Add new tables and columns for criteria subscriptions   --------------------------
----------------------------------------------------------------------------------------

-------------------------------------
-- Table : SubscriptionCriteriaMatch
-------------------------------------

IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id = object_id('SubscriptionCriteriaMatch'))

	CREATE TABLE [dbo].[SubscriptionCriteriaMatch](
		[SubscriptionCriteriaMatchId] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
		[SubscriptionId] [uniqueidentifier] NOT NULL,
		[DataChangeEventId] [uniqueidentifier] NOT NULL,
		CONSTRAINT [PK_SubscriptionCriteriaMatch] PRIMARY KEY CLUSTERED ( [SubscriptionCriteriaMatchId] ASC) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
		CONSTRAINT [FK_SubscriptionCriteriaMatch_DataChangeEvent] FOREIGN KEY([DataChangeEventId]) REFERENCES [dbo].[DataChangeEvent] ([DataChangeEventId]) ON UPDATE CASCADE ON DELETE CASCADE,
		CONSTRAINT [FK_SubscriptionCriteriaMatch_Subscription] FOREIGN KEY([SubscriptionId]) REFERENCES [dbo].[Subscription] ([SubscriptionId]) ON UPDATE CASCADE ON DELETE CASCADE
	) ON [PRIMARY]

GO

---------------------------
-- Table : DataChangeEvent
---------------------------

IF NOT EXISTS (SELECT * FROM sys.columns c WHERE name = 'PersonRegistrationId' and object_id=object_id('DataChangeEvent'))
	ALTER TABLE dbo.DataChangeEvent ADD PersonRegistrationId UNIQUEIDENTIFIER NOT NULL
GO

------------------------
-- Table : Subscription
------------------------

IF NOT EXISTS (SELECT * FROM sys.columns c WHERE name = 'Criteria' and object_id=object_id('Subscription'))
	ALTER TABLE dbo.Subscription ADD Criteria XML NULL DEFAULT NULL
GO

IF NOT EXISTS (SELECT * FROM sys.columns c WHERE name = 'Created' and object_id=object_id('Subscription'))
	ALTER TABLE dbo.Subscription ADD Created Datetime NOT NULL DEFAULT '1900-01-01 00:00:00'
GO

IF NOT EXISTS (SELECT * FROM sys.columns c WHERE name = 'Deactivated' and object_id=object_id('Subscription'))
	ALTER TABLE dbo.Subscription ADD Deactivated Datetime NULL DEFAULT NULL
GO

IF NOT EXISTS (SELECT * FROM sys.columns c WHERE name = 'LastCheckedUUID' and object_id=object_id('Subscription'))
	ALTER TABLE dbo.Subscription ADD LastCheckedUUID uniqueidentifier NULL DEFAULT NULL
GO

------------------------------
-- Table : SubscriptionPerson
------------------------------

IF NOT EXISTS (SELECT * FROM sys.columns c WHERE name = 'Created' and object_id=object_id('SubscriptionPerson'))
	ALTER TABLE dbo.SubscriptionPerson ADD Created Datetime NOT NULL DEFAULT '1900-01-01 00:00:00'
GO

IF NOT EXISTS (SELECT * FROM sys.columns c WHERE name = 'Removed' and object_id=object_id('SubscriptionPerson'))
	ALTER TABLE dbo.SubscriptionPerson ADD Removed Datetime NULL DEFAULT NULL
GO

-----------------------------
-- Table : EventNotification
-----------------------------

IF NOT EXISTS (SELECT * FROM sys.columns c WHERE name = 'IsLastNotification' and object_id=object_id('EventNotification'))
	ALTER TABLE dbo.EventNotification ADD IsLastNotification bit NULL DEFAULT NULL
GO


----------------------------------------------------------------------------------------
---   Stored procedures   --------------------------------------------------------------
----------------------------------------------------------------------------------------

--====================================
--EnqueueDataChangeEventNotifications
--====================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EnqueueDataChangeEventNotifications]') AND type in (N'P', N'PC'))
	DROP PROCEDURE dbo.EnqueueDataChangeEventNotifications
GO

CREATE Procedure EnqueueDataChangeEventNotifications
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
GO


--====================================
-- UpdatePersonLists
--====================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdatePersonLists]') AND type in (N'P', N'PC'))
	DROP PROCEDURE dbo.UpdatePersonLists
GO

CREATE PROCEDURE [dbo].[UpdatePersonLists]
	@Now datetime,
	@SubscriptionTypeId Int
AS
	
	---------------------------------------------------------------------------------------------------
	--- Create a temp table with the changed persons and their in/out status before and after the change
	---------------------------------------------------------------------------------------------------
	DECLARE @TMP TABLE(SubscriptionId uniqueidentifier, PersonUuid uniqueidentifier, PersonRegistrationId uniqueidentifier, SubscriptionPersonId uniqueidentifier, SubscriptionCriteriaMatchId uniqueidentifier)
	
	INSERT INTO 
		@TMP 
	SELECT
		S_DCE.SubscriptionId,
		S_DCE.PersonUuid, 
		S_DCE.PersonRegistrationId,
		SP.SubscriptionPersonId,
		SCM.SubscriptionCriteriaMatchId
	FROM
		(
			SELECT * FROM Subscription S, DataChangeEvent DCE
			WHERE S.SubscriptionTypeId = @SubscriptionTypeId AND S.Criteria IS NOT NULL AND S.Deactivated IS NULL
		) AS S_DCE
	LEFT OUTER JOIN SubscriptionPerson SP			ON S_DCE.SubscriptionId = SP.SubscriptionId		AND	S_DCE.PersonUuid		= SP.PersonUuid
	LEFT OUTER JOIN SubscriptionCriteriaMatch SCM	ON S_DCE.SubscriptionId = SCM.SubscriptionId	AND	S_DCE.DataChangeEventId	= SCM.DataChangeEventId
	WHERE
		SP.Removed IS NULL

	---------------------------------------------------------------------------------------------------
	--- Insert persons that have now changed to be in the criteria
	---------------------------------------------------------------------------------------------------
	INSERT INTO SubscriptionPerson(SubscriptionId, PersonUuid, Created)
	SELECT SubscriptionId, PersonUuid, @Now
	FROM @TMP T
	WHERE 
			T.SubscriptionPersonId IS NULL 
		AND SubscriptionCriteriaMatchId IS NOT NULL


	---------------------------------------------------------------------------------------------------
	--- Remove persons that no longer match the subscription's criteria
	---------------------------------------------------------------------------------------------------
	UPDATE 
		SP
	SET 
		Removed = 1
	FROM 
		SubscriptionPerson SP INNER JOIN @TMP T 
		ON 
			T.SubscriptionPersonId = SP.SubscriptionPersonId
	WHERE 
		-- No need for T.SubscriptionPersonId IS NOT NULL because it is implicitly included in the join condition
		T.SubscriptionCriteriaMatchId IS NULL

	
	---------------------------------------------------------------------------------------------------
	--- Explicitly enqueue the last notification for persons that no longer match the criteria
	---------------------------------------------------------------------------------------------------
	INSERT INTO 
		EventNotification (SubscriptionId, PersonUuid, CreatedDate, IsLastNotification)
	SELECT 
		SubscriptionId, PersonUuid, @Now, 1
	FROM 
		@TMP
	WHERE 
			SubscriptionPersonId IS NOT NULL 
		AND SubscriptionCriteriaMatchId IS NULL


	---------------------------------------------------------------------------------------------------
	--- Delete based on inner join, to only affect the records that have been taken into account
	--- This way the procedure is thread safe regarding this table
	--- We still may need to make it thread safe regarding other tables (Subscription, SubscriptionPerson)
	---------------------------------------------------------------------------------------------------
	DELETE 
		SCM
	FROM 
		SubscriptionCriteriaMatch SCM INNER JOIN @TMP TMP
		ON SCM.SubscriptionCriteriaMatchId = TMP.SubscriptionCriteriaMatchId


RETURN 0