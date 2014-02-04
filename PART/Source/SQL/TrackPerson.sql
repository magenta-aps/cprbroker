/*	
	====================================================================================
	=====  Getting a trace of external calls (including setting of subscriptions)  =====
	====================================================================================
*/

declare @CprNumber varchar(10)
SET @CprNumber = '1234567890'

DECLARE @FromDate DATETIME, @ToDate DateTime

/*
	TODO:// Modify these date to allow viewing of logs older than 1 month
*/
SET @FromDate = DATEADD(MONTH, -1, GETDATE())
SET @ToDate = GETDATE()


/*	
	=========================================================
	=====  Get Calls made after installation of 2.2.2  =====
	=========================================================
*/
SELECT * 
FROM DataProviderCall dpe
WHERE Input = @CprNumber

-- To get the application name, use the value of AvtivityIdColumn in DataProviderCall
DECLARE @ActivityIds TABLE (ActivityId UNIQUEIDENTIFIER)

INSERT INTO @ActivityIds
SELECT ActivityId
FROM DataProviderCall dpe
WHERE Input = @CprNumber

SELECT a.Name, le.*
FROM	@ActivityIds aid
	INNER JOIN LogEntry le ON le.ActivityId = aid.ActivityId
	INNER JOIN Application a ON le.ApplicationId = a.ApplicationId
WHERE 
	LogDate BETWEEN @FromDate AND @ToDate



/*	
	=========================================================
	=====  Get Calls made before installation of 2.2.2  =====
	=========================================================
*/

SELECT	a.Name,le.* 
FROM	LogEntry le
		INNER JOIN Application a ON le.ApplicationId = a.ApplicationId
WHERE 
		LogDate BETWEEN @FromDate AND @ToDate -- REMOVE THIS TO LOOK INTO THE FULL LOG
		
		AND 
		(
				Text = 'Calling DPR Diversion : ' + @CprNumber    -- DPR Diversion
			OR	Text = 'Calling AS78205 with PNR ' + @CprNumber   -- KMD
			OR  Text = 'Calling AS78207 with PNR ' + @CprNumber  -- KMD
			OR	Text LIKE 'CPR client: PNR <' + @CprNumber + '>%' -- CPR Direct
		)

/*	
	===================================
	=====  Get the person's UUID  =====
	===================================
*/

DECLARE @UUID UNIQUEIDENTIFIER
SELECT	@UUID = UUID 
FROM	PersonMapping 
WHERE	CprNumber = @CprNumber

-- Registrations
SELECT	* 
FROM	PersonRegistration
WHERE	UUID = @UUID 
ORDER BY RegistrationDate DESC, BrokerUpdateDate DESC