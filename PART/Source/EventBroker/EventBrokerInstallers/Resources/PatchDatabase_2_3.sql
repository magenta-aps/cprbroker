
-----------------------------------------------------------------------------------------------
-------------------   Add identity column DataChangeEvent.ReceivedOrder   ---------------------
-----------------------------------------------------------------------------------------------

-- Create temp table if not already there
IF NOT EXISTS (SELECT * FROM sys.tables where name='DataChangeEvent_TMP')
	CREATE TABLE DataChangeEvent_TMP (DataChangeEventId uniqueidentifier, PersonUuid uniqueidentifier, PersonRegistrationId uniqueidentifier, DueDate datetime, ReceivedDate DateTime)
GO

-- Copy existing data changes to tampe table
INSERT INTO DataChangeEvent_TMP (DataChangeEventId , PersonUuid , PersonRegistrationId , DueDate , ReceivedDate) 
SELECT DataChangeEventId , PersonUuid , PersonRegistrationId , DueDate , ReceivedDate 
FROM DataChangeEvent

GO

-- Delete from original table
DELETE DataChangeEvent
GO

-- Add identity column ReceivedOrder if not already there
IF NOT EXISTS (SELECT * FROM sys.columns c INNER JOIN sys.tables t ON c.object_id = t.object_id where t.name='DataChangeEvent' AND c.name = 'ReceivedOrder')
	ALTER TABLE dbo.DataChangeEvent 
	ADD	ReceivedOrder int NOT NULL IDENTITY (1, 1)
GO

-- Copy back into original table
INSERT INTO DataChangeEvent (DataChangeEventId , PersonUuid , PersonRegistrationId , DueDate , ReceivedDate) 
SELECT DISTINCT DataChangeEventId , PersonUuid , PersonRegistrationId , DueDate , ReceivedDate 
FROM DataChangeEvent_TMP
ORDER BY ReceivedDate

GO

-- Now drop temp table
IF EXISTS (SELECT * FROM sys.tables where name='DataChangeEvent_TMP')
	DROP TABLE DataChangeEvent_TMP
GO


-----------------------------------------------------------------------------------------------
-------------------------------   Drop legacy stored procedure  -------------------------------
-----------------------------------------------------------------------------------------------

IF EXISTS (SELECT * FROM sys.procedures WHERE name='InsertChangeNotificationData')
	DROP PROCEDURE InsertChangeNotificationData
GO

-----------------------------------------------------------------------------------------------
------------------------------   New column Subscription.Ready  -------------------------------
-----------------------------------------------------------------------------------------------

IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'Ready' AND object_id = OBJECT_ID('Subscription'))
	ALTER TABLE Subscription ADD Ready BIT NOT NULL DEFAULT 0
GO

UPDATE Subscription SET Ready = 1 WHERE Ready IS NULL AND LastCheckedUUID IS NULL
GO