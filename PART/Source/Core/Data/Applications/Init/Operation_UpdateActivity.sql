IF EXISTS (SELECT * FROM sys.triggers WHERE name = N'Operation_UpdateActivity')
	DROP TRIGGER Operation_UpdateActivity
GO

CREATE TRIGGER Operation_UpdateActivity
ON Operation
FOR INSERT, UPDATE
AS
	SET NOCOUNT ON

	UPDATE a
	SET HasOperations = 1
	FROM Activity a INNER JOIN INSERTED ins ON a.ActivityId = ins.ActivityId

	SET NOCOUNT OFF