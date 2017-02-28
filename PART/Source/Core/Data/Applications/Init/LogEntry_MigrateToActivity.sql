DECLARE @MaxDate DATETIME, @TempMaxDate DATETIME, @TempMinDate DATETIME, @IntervalHours INT
DECLARE @msg VARCHAR(max)

SET @IntervalHours = 48
SET @MaxDate = DATEADD(YEAR, 0, GETDATE())
SELECT  @TempMinDate = MIN(LogDate) FROM LogEntry
SET @TempMaxDate = DATEADD(HOUR, @IntervalHours, @TempMinDate)		
	

WHILE @TempMinDate < @MaxDate
BEGIN
	-- Insert rows into Activity, if not already there
	WITH l AS 
	(
		SELECT le.ActivityId, le.ApplicationId, le.LogDate, le.UserToken, le.UserId, le.MethodName, 
		ROW_NUMBER() OVER (PARTITION BY le.ActivityId ORDER BY LogDate ASC) AS RN
		FROM LogEntry le LEFT OUTER JOIN Activity act 
		ON le.ActivityId = act.ActivityId AND act.StartTS BETWEEN DATEADD(HOUR, -3, le.LogDate) AND DATEADD(HOUR, 3, le.LogDate)
		WHERE le.LogDate BETWEEN @TempMinDate AND @TempMaxDate
		AND act.StartTS IS NULL
	)
	INSERT INTO Activity (ActivityId, ApplicationId, StartTS, UserToken, UserId, MethodName)
	SELECT ActivityId, ApplicationId, LogDate, UserToken, UserId, MethodName
	FROM l
	WHERE RN = 1

	set @msg = N'Selected records between ' + CONVERT(varchar, @TempMinDate, 121) + ' AND ' + CONVERT(varchar, @TempMaxDate, 121) + ' rows : ' + convert(varchar, @@ROWCOUNT);
	RAISERROR (@msg, 0, 1) WITH NOWAIT
	
	-- Trigger update of relevant statistics in Activity
	UPDATE LogEntry SET MethodName = MethodName WHERE LogDate BETWEEN @TempMinDate AND @TempMaxDate

	SET @TempMinDate = @TempMaxDate
	SET @TempMaxDate = DATEADD(HOUR, @IntervalHours, @TempMinDate)		
	
END