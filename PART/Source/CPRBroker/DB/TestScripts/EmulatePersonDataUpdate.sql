IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'EmulatePersonDataUpdate')
	BEGIN
		DROP  Procedure  EmulatePersonDataUpdate
	END

GO

CREATE Procedure EmulatePersonDataUpdate
(
	@MaxPersons INT
)

AS
	CREATE TABLE #PersonId (PersonId UNIQUEIDENTIFIER)
	DECLARE @SQL VARCHAR(MAX)
	SET @SQL = 'INSERT INTO #PersonId (PersonId) SELECT TOP ' + CAST (@MaxPersons AS VARCHAR(20)) + ' PersonId FROM Person'
	EXEC (@SQL)

	DECLARE @FakeBirthdate DATETIME
	SET @FakeBirthdate = GETDATE()
	SET @FakeBirthdate = DATEADD(hour, - (DATEPART(hour,@FakeBirthdate)),@FakeBirthdate)
	SET @FakeBirthdate = DATEADD(minute, - (DATEPART(minute,@FakeBirthdate)),@FakeBirthdate)
	SET @FakeBirthdate = DATEADD(second, - (DATEPART(second,@FakeBirthdate)),@FakeBirthdate)
	SET @FakeBirthdate = DATEADD(millisecond, - (DATEPART(millisecond,@FakeBirthdate)),@FakeBirthdate)
	
	UPDATE P
	SET BirthDate = DATEADD(DAY, 1, ISNULL(BirthDate, @FakeBirthdate))
	FROM Person P
	INNER JOIN #PersonId TMP ON P.PersonId = TMP.PersonId

GO


