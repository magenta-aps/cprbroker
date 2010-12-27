IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'EnqueueDataChangeEventNotifications')
	BEGIN
		DROP  Procedure  EnqueueDataChangeEventNotifications
	END

GO

CREATE Procedure EnqueueDataChangeEventNotifications
(
		@StartDate DateTime,
		@EndDate DateTime,
		@Now DateTime,
		@SubscriptionTypeId Int
)


AS

	-- Subscriptions for specific persons
	INSERT INTO EventNotification (SubscriptionId, PersonUUID, CreatedDate)
	SELECT S.SubscriptionId, DCE.UUID, @Now
	FROM DataChangeEvent AS DCE
	INNER JOIN SubscriptionPerson AS SP ON SP.PersonId = DCE.UUID
	INNER JOIN Subscription AS S ON S.SubscriptionId = SP.SubscriptionId
	WHERE 
		DCE.ReceivedDate BETWEEN @StartDate AND @EndDate
		AND S.IsForAllPersons = 0
		AND S.SubscriptionTypeId = @SubscriptionTypeId
		
	-- Subscriptions for all persons
	INSERT INTO EventNotification (SubscriptionId, PersonUUID, CreatedDate)
	SELECT S.SubscriptionId, DCE.UUID, @Now
	FROM DataChangeEvent AS DCE,	Subscription AS S	
	WHERE 
		DCE.ReceivedDate BETWEEN @StartDate AND @EndDate
		AND S.IsForAllPersons = 1
		AND S.SubscriptionTypeId = @SubscriptionTypeId
		
	

GO


