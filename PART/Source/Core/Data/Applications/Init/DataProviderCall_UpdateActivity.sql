IF EXISTS (SELECT * FROM sys.triggers WHERE name = N'DataProviderCall_UpdateActivity')
	DROP TRIGGER DataProviderCall_UpdateActivity
GO

CREATE TRIGGER DataProviderCall_UpdateActivity
ON DataProviderCall
FOR INSERT, UPDATE
AS
	SET NOCOUNT ON

	UPDATE a
	SET HasDataProviderCalls = 1
	FROM Activity a INNER JOIN INSERTED ins ON a.ActivityId = ins.ActivityId

	SET NOCOUNT OFF