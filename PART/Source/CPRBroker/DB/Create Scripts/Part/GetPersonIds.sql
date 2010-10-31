IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'GetPersonIds')
	BEGIN
		DROP  Procedure  GetPersonIds
	END

GO

CREATE Procedure GetPersonIds
/*
	(
		@parameter1 int = 5,
		@parameter2 datatype OUTPUT
	)

*/
AS


GO

/*
GRANT EXEC ON Stored_Procedure_Name TO PUBLIC

GO
*/

