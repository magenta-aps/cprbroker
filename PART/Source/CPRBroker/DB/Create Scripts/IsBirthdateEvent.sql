/*
	Determines whether the given parameters are a match for raising a birthdate event notification
*/
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'FN' AND name = 'IsBirthdateEvent')
	BEGIN
		DROP  Function IsBirthdateEvent
	END

GO

CREATE Function IsBirthdateEvent
(
		@Now DateTime,
		@BirthDate DateTime,
		@AgeYears INT,
		@PriorDays INT
)
RETURNS BIT
AS
BEGIN
	IF
	(	
		-- Exact age match
		@AgeYears IS NOT NULL 
		AND dateadd (day, @PriorDays, @Now) = dateadd(year, @AgeYears, @BirthDate)
	)
	OR
	(
		-- Any age match
		@AgeYears IS NULL 
		AND DATEPART(DAY,DATEADD(day, @PriorDays, @Now)) = DATEPART(DAY,@BirthDate)
		AND DATEPART(MONTH,DATEADD(day, @PriorDays, @Now)) = DATEPART(MONTH,@BirthDate)
	)
		RETURN 1
		
	RETURN 0
END

GO



