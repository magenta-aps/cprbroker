/*
	Gets the list of subscriptions that should be notified at a given date
*/
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'GetDueNotifications')
	BEGIN
		DROP  Procedure  GetDueNotifications
	END

GO

CREATE Procedure GetDueNotifications
(
		@Now datetime,
		@LastTime datetime
)

AS
	-- Select all the subscriptions that should be fired
	SELECT S.*
	FROM Application AS A 
	INNER JOIN Subscription AS S ON S.ApplicationId = A.ApplicationId
	INNER JOIN Channel AS C ON C.SubscriptionId = S.SubscriptionId
	LEFT OUTER JOIN BirthdateSubscription AS BDS ON BDS.SubscriptionId = S.SubscriptionId	
	LEFT OUTER JOIN DataSubscription AS DS ON DS.SubscriptionId = S.SubscriptionId	
	WHERE 
	  A.IsApproved = 1
	AND 
	  EXISTS (
		SELECT P.PersonUuid
		FROM Person AS P
		WHERE 
		-- Person match
		(
			S.IsForAllPersons = 1 
			OR EXISTS (
				SELECT PS.SubscriptionPersonId 
				FROM SubscriptionPerson AS PS
				WHERE PS.SubscriptionId = S.SubscriptionId AND PS.PersonUuid = P.PersonUuid
				)
		)
		
		AND 
		(
			-- Match birthdate
			(
				BDS.SubscriptionId IS NOT NULL 
				AND dbo.IsBirthdateEvent(@Now, P.BirthDate, BDS.AgeYears, BDS.PriorDays) = 1		
			)
			OR			
			-- Match data change
			(
				DS.SubscriptionId IS NOT NULL
				AND 
				(
					dbo.IsDataChangeEvent(P.PersonUuid, @Now, @LastTime) = 1
				)
			)
		)
		
		
	)
	

GO

