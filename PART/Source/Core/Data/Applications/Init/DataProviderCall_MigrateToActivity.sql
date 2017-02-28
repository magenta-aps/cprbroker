DECLARE @MaxDate DATETIME, @TempMaxDate DATETIME, @TempMinDate DATETIME, @IntervalHours INT
DECLARE @msg VARCHAR(max)

SET @IntervalHours = 24 * 30
SET @MaxDate = DATEADD(YEAR, 0, GETDATE())
SELECT  @TempMinDate = MIN(CallTime) FROM DataProviderCall
SET @TempMaxDate = DATEADD(HOUR, @IntervalHours, @TempMinDate)		
	

WHILE @TempMinDate < @MaxDate
BEGIN
	-- Insert rows into Activity, if not already there
	WITH l AS 
	(
		SELECT dpc.ActivityId, NULL as ApplicationId, dpc.CallTime, NULL as UserToken, NULL as UserId, NULL as MethodName, 
		ROW_NUMBER() OVER (PARTITION BY dpc.ActivityId ORDER BY CallTime ASC) AS RN
		FROM DataProviderCall dpc LEFT OUTER JOIN Activity act 
		ON dpc.ActivityId = act.ActivityId AND act.StartTS BETWEEN DATEADD(HOUR, -3, dpc.CallTime) AND DATEADD(HOUR, 3, dpc.CallTime)
		WHERE dpc.CallTime BETWEEN @TempMinDate AND @TempMaxDate
		AND act.StartTS IS NULL
	)
	INSERT INTO Activity (ActivityId, ApplicationId, StartTS, UserToken, UserId, MethodName)
	SELECT ActivityId, ApplicationId, CallTime, UserToken, UserId, MethodName
	FROM l
	WHERE RN = 1

	set @msg = N'Selected records between ' + CONVERT(varchar, @TempMinDate, 121) + ' AND ' + CONVERT(varchar, @TempMaxDate, 121) + ' rows : ' + convert(varchar, @@ROWCOUNT);
	RAISERROR (@msg, 0, 1) WITH NOWAIT

	-- Trigger update of relevant statistics in Activity
	UPDATE  DataProviderCall SET Cost = Cost WHERE CallTime BETWEEN @TempMinDate AND @TempMaxDate


	SET @TempMinDate = @TempMaxDate
	SET @TempMaxDate = DATEADD(HOUR, @IntervalHours, @TempMinDate)		
	
END