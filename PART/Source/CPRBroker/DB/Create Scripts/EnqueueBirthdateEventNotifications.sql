/*
	Checks whether a birthdate subscription should fire a notification
	If so, creates a new row in Notification table with its child rows
*/
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'EnqueueBirthdateEventNotifications')
	BEGIN
		DROP  Procedure  EnqueueBirthdateEventNotifications
	END

GO

CREATE Procedure EnqueueBirthdateEventNotifications
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
	CREATE TABLE #Person (EventNotificationId UNIQUEIDENTIFIER DEFAULT NEWID(), PersonUuid UNIQUEIDENTIFIER, Birthdate DATETIME, Age INT)
		
	
	-- Search  for persons that match the subscription rule
	INSERT INTO #Person (PersonUuid, Birthdate, Age)
	SELECT PB.PersonUuid, PB.BirthDate, DATEDIFF(YEAR, PB.Birthdate, DATEADD(day, @PriorDays, @Today))	
	FROM PersonBirthdate AS PB
	LEFT OUTER JOIN SubscriptionPerson AS SP ON PB.PersonUuid = SP.PersonUuid
	WHERE 
	(
		@IsForAllPersons = 1 OR SP.SubscriptionId = @SubscriptionId
	)
	AND
	(
		dbo.IsBirthdateEvent(@Today, PB.BirthDate, @AgeYears, @PriorDays) = 1
	)
	
	
	INSERT INTO EventNotification (EventNotificationId, SubscriptionId, PersonUUID, CreatedDate)
	SELECT EventNotificationId, @SubscriptionId, PersonUuid, @Today
	FROM #Person
		
	INSERT INTO BirthdateEventNotification (EventNotificationId, Age)
	SELECT EventNotificationId, Age
	FROM #Person
		
	

GO


