/*
	Checks whether a birthdate subscription should fire a notification
	If so, creates a new row in Notification table with its child rows
*/
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'InsertBirthdateNotificationData')
	BEGIN
		DROP  Procedure  InsertBirthdateNotificationData
	END

GO

CREATE Procedure InsertBirthdateNotificationData
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
	CREATE TABLE #Person (NotificationPersonID UNIQUEIDENTIFIER DEFAULT NEWID(), PersonID UNIQUEIDENTIFIER, Birthdate DATETIME, Age INT)
		
	
	-- Search  for persons that match the subscription rule
	INSERT INTO #Person (PersonId, Birthdate, Age)
	SELECT PD.UUID, PD.BirthDate, DATEDIFF(YEAR, PD.Birthdate, DATEADD(day, @PriorDays, @Today))
	FROM 
	(
		SELECT DISTINCT P.UUID, PA.Birthdate
		FROM Person AS P
		INNER JOIN PersonRegistration AS PR ON P.UUID = PR.UUID
		INNER JOIN PersonAttributes AS PA ON PA.PersonRegistrationID = PR.PersonRegistrationId
		WHERE PA.BirthDate IS NOT NULL
	) AS PD
	LEFT OUTER JOIN SubscriptionPerson AS SP ON PD.UUID = SP.PersonId
	WHERE 
	(
		@IsForAllPersons = 1 OR SP.SubscriptionId = @SubscriptionId
	)
	AND
	(
		dbo.IsBirthdateEvent(@Today, PD.BirthDate, @AgeYears, @PriorDays) = 1
	)
	
	IF EXISTS (SELECT * FROM #Person)-- If persons are found
	BEGIN
		-- Insert Notification row and child rows
		SET @NotificationID = NEWID()
		
		INSERT INTO Notification (NotificationId, SubscriptionId, NotificationDate)
		VALUES (@NotificationId, @SubscriptionId, @Today)
		
		INSERT INTO BirthdateNotification (NotificationID, AgeYears, PriorDays)
		VALUES (@NotificationId, @AgeYears, @PriorDays)
		
		INSERT INTO NotificationPerson (NotificationPersonId, NotificationId, PersonId)
		SELECT NotificationPersonId, @NotificationId, PersonId
		FROM #Person
		
		INSERT INTO BirthdateNotificationPerson(NotificationPersonID, Age)
		SELECT P.NotificationPersonID, P.Age
		FROM #Person P
		INNER JOIN NotificationPerson NP ON P.NotificationPersonID = NP.NotificationPersonID
	END
	
	-- Finally, select the new Notification row
	SELECT * 
	FROM Notification	
	WHERE NotificationId = @NotificationId

GO


