CREATE PROCEDURE [dbo].[UpdatePersonList]
	@SubscriptionId uniqueidentifier,
	@Now datetime
AS
	DECLARE @EmptyUuid uniqueidentifier;
	SET @EmptyUuid = '{00000000-0000-0000-0000-000000000000}'

	---------------------------------------------------------------------------------------------------
	--- Create a temp table with the changed persons and their in/out status before and after the change
	---------------------------------------------------------------------------------------------------
	DECLARE @TMP TABLE(PersonUuid uniqueidentifier, PersonRegistrationId uniqueidentifier, Old bit, New bit)
	
	INSERT INTO @TMP SELECT
		DCE.PersonUuid, 
		DCE.PersonRegistrationId, 
		CASE(ISNULL(SP.SubscriptionId, @EmptyUuid)) WHEN @EmptyUuid THEN 0 ELSE 1 END, 
		CASE(ISNULL(TSP.DataChangeEventId, @EmptyUuid)) WHEN @EmptyUuid THEN 0 ELSE 1 END			
	FROM 
		DataChangeEvent DCE 
	LEFT OUTER JOIN SubscriptionPerson SP ON DCE.PersonUuid = SP.PersonUuid
	LEFT OUTER JOIN TempSubscriptionPerson TSP ON DCE.DataChangeEventId = TSP.DataChangeEventId
	WHERE
		SP.SubscriptionId = @SubscriptionId
	AND TSP.SubscriptionId = @SubscriptionId
	AND SP.Removed IS NULL

	---------------------------------------------------------------------------------------------------
	--- Insert persons that have now changed to be in the criteria
	---------------------------------------------------------------------------------------------------
	INSERT INTO SubscriptionPerson(SubscriptionId, PersonUuid, Created)
	SELECT @SubscriptionId, PersonUuid, @Now
	FROM @TMP T
	WHERE Old = 0 AND New = 1

	---------------------------------------------------------------------------------------------------
	--- Remove persons that no longer match the subscription's criteria
	---------------------------------------------------------------------------------------------------
	UPDATE SP
	SET Removed = 1
	FROM SubscriptionPerson SP
	INNER JOIN @TMP T ON T.PersonUuid = SP.PersonUuid
	WHERE SP.SubscriptionId = @SubscriptionId
	AND T.Old = 1 AND T.New = 0

	
	---------------------------------------------------------------------------------------------------
	--- Explicitly enqueue the last notification for persons that no longer match the criteria
	---------------------------------------------------------------------------------------------------
	INSERT INTO EventNotification (SubscriptionId, PersonUuid, CreatedDate, IsLastNotification)
	SELECT @SubscriptionId, T.PersonUuid, @Now, 1
	FROM SubscriptionPerson SP
	INNER JOIN @TMP T ON T.PersonUuid = SP.PersonUuid
	WHERE SP.SubscriptionId = @SubscriptionId
	AND T.Old = 1 AND T.New = 0

RETURN 0