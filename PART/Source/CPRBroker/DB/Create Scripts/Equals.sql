/*
	Determines whether two objects has the same value 
	Similar to Object.Equals() in .NET
	Returns true if both have same value or both are null
	False otherwise
*/
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'FN' AND name = 'Equals')
	BEGIN
		DROP  Function Equals
	END

GO

CREATE Function Equals
(
	@Value1 sql_variant,
	@Value2 sql_variant
)
RETURNS BIT
AS
BEGIN
	IF @Value1 IS NULL AND @Value2 IS NULL 
		RETURN 1
	ELSE IF @Value1 IS NULL OR @Value2 IS NULL 
		RETURN 0 
	ELSE IF @Value1 = @Value2 
		RETURN 1
	
	RETURN 0
END