IF EXISTS (SELECT * FROM sys.triggers WHERE name = N'LogEntry_UpdateActivity')
	DROP TRIGGER LogEntry_UpdateActivity
GO

CREATE TRIGGER LogEntry_UpdateActivity
ON LogEntry
FOR INSERT, UPDATE
AS
	SET NOCOUNT ON

	UPDATE a
	SET HasCriticalErrors = 1
	FROM Activity a INNER JOIN INSERTED ins ON a.ActivityId = ins.ActivityId
	WHERE ins.LogTypeId = 1

	UPDATE a
	SET HasWarnings = 1
	FROM Activity a INNER JOIN INSERTED ins ON a.ActivityId = ins.ActivityId
	WHERE ins.LogTypeId = 2

	UPDATE a
	SET HasErrors = 1
	FROM Activity a INNER JOIN INSERTED ins ON a.ActivityId = ins.ActivityId
	WHERE ins.LogTypeId = 4

	UPDATE a
	SET HasInformation = 1
	FROM Activity a INNER JOIN INSERTED ins ON a.ActivityId = ins.ActivityId
	WHERE ins.LogTypeId = 8


	SET NOCOUNT OFF