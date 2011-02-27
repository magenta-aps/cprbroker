/*
	Checks if a data change subscription should be fired on a date
	If so, Inserts the necessary row in the Notification table and its child rows
*/

IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'InsertChangeNotificationData')
	BEGIN
		DROP  Procedure  InsertChangeNotificationData
	END

GO

CREATE Procedure InsertChangeNotificationData
(
	@SubscriptionId UNIQUEIDENTIFIER,
	@Today DATETIME,
	@LastTime DATETIME
)

AS
	DECLARE @NotificationId UNIQUEIDENTIFIER
	DECLARE @IsForAllPersons BIT
	
	-- Select subscription data
	SELECT @IsForAllPersons= IsForAllPersons
	FROM Subscription AS S 
	INNER JOIN DataSubscription AS DS ON S.SubscriptionId = DS.SubscriptionId
	WHERE S.SubscriptionId = @SubscriptionId
	
	-- Use this table to store temporary data
	CREATE TABLE #Person (NotificationPersonId UNIQUEIDENTIFIER DEFAULT NEWID(), PersonUuid UNIQUEIDENTIFIER)
	
	-- Search for persons that could possible fire the notification
	INSERT INTO #Person (PersonUuid)
	SELECT P.PersonUuid
	FROM Person AS P
	LEFT OUTER JOIN SubscriptionPerson AS SP ON P.PersonUuid = SP.PersonUuid
	WHERE 
	(
		@IsForAllPersons = 1 OR SP.SubscriptionId = @SubscriptionId
	)
	AND
	(
		dbo.IsDataChangeEvent(P.PersonUuid, @Today, @LastTime) = 1
	)
	
	
	IF EXISTS (SELECT TOP 1 * FROM #Person) -- If persons are found
	BEGIN
		-- Insert data into physical tables
		SET @NotificationID = NEWID()
		
		INSERT INTO Notification (NotificationId, SubscriptionId, NotificationDate)
		VALUES (@NotificationId, @SubscriptionId, @Today)
		
		INSERT INTO NotificationPerson (NotificationId, NotificationPersonId, PersonUuid)
		SELECT @NotificationId, NotificationPersonId, PersonUuid
		FROM #Person		
		
	END
	
	-- Finally, return the new Notification object
	SELECT * 
	FROM Notification 
	WHERE NotificationId = @NotificationId
GO