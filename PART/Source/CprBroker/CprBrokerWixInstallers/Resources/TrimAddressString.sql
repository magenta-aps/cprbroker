/*
    Trims strings that are used in address numbers
    - Removed spaces and zeros from the left
    - Removes spaces from the right
*/

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'FN' AND name = 'TrimAddressString')
	DROP FUNCTION TrimAddressString
GO

CREATE FUNCTION TrimAddressString(@s VARCHAR(MAX))
    RETURNS VARCHAR(MAX)
AS
BEGIN
    DECLARE @i int, @l INT
    SET @i = 0;
	SET @l = LEN(@s)

    WHILE SUBSTRING(@s,@i+1,1) in ('0', ' ') AND @i < @l
	    SET @i = @i + 1

    RETURN RTRIM(SUBSTRING (@s, @i+1, @l - @i))
END
GO