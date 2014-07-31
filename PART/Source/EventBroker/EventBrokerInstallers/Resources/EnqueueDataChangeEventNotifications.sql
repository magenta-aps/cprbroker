/*
    Input:
    ------
    - Current time (@Now) to mark changes with timestamp
    - Subset of data change events (defined as ReceivedOrder <= @LatestReceivedOrder)
    - @SubscriptionTypeId to mark data change subscriptions (instead of having the typeId hardcoded)

    Result:
    -------
    Rows inserted in EventNotification for each DataChangeEvent X active subscriptions that are any of these:
    - Are for specific persons, and these persons include the person in the DataChangeEvent
        (Including criteria subscriptions, as their SubscriptionPerson rows should have been updated in previous steps)
    - Are for all persons

    Output:
    -------
    N/A
*/

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'EnqueueDataChangeEventNotifications')
	DROP PROCEDURE EnqueueDataChangeEventNotifications
GO

CREATE Procedure EnqueueDataChangeEventNotifications
(
    @Now DateTime,
    @LatestReceivedOrder Int,
    @SubscriptionTypeId Int
)

AS
    -- Subscriptions for specific persons or criteria
    INSERT INTO 
        EventNotification (SubscriptionId, PersonUUID, CreatedDate, IsLastNotification)
    SELECT 
        S.SubscriptionId, DCE.PersonUuid, @Now, 0
    FROM 
        DataChangeEvent AS DCE
        INNER JOIN SubscriptionPerson AS SP ON SP.PersonUuid = DCE.PersonUuid
        INNER JOIN Subscription AS S ON S.SubscriptionId = SP.SubscriptionId
    WHERE 
        -- Conditions for DataChangeEvent
            DCE.ReceivedOrder <= @LatestReceivedOrder
            
        -- Conditions for SubscriptionPerson
        AND SP.Removed IS NULL
            
        -- Conditions for Subscription
        AND S.IsForAllPersons = 0
        AND S.SubscriptionTypeId = @SubscriptionTypeId
        AND S.Ready = 1
        AND S.Deactivated IS NULL



    -- Subscriptions for all persons
    INSERT INTO EventNotification (SubscriptionId, PersonUUID, CreatedDate, IsLastNotification)
    SELECT S.SubscriptionId, DCE.PersonUuid, @Now, 0
    FROM DataChangeEvent AS DCE,    Subscription AS S    
    WHERE 
        -- Conditions for DataChangeEvent
            DCE.ReceivedOrder <= @LatestReceivedOrder
            
        -- Conditions for Subscription
        AND S.IsForAllPersons = 1
        AND S.SubscriptionTypeId = @SubscriptionTypeId
        AND S.Ready = 1
        AND S.Deactivated IS NULL
GO
