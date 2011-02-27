IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'CreateSynonym')
	BEGIN
		DROP  Procedure  CreateSynonym
	END

GO

CREATE Procedure CreateSynonym
(
		@Synonym VARCHAR(50),
		@Server VARCHAR(50),
		@Database VARCHAR(50),
		@Schema VARCHAR(50),
		@Table VARCHAR(50)
)


AS
	DECLARE @SQL VARCHAR(MAX)
	SET @SQL = 'CREATE SYNONYM '
		+ '[' + @Synonym + ']'
		+ ' FOR ' 
		+ '[' + @Server + '].'
		+ '[' + @Database + '].'
		+ '[' + @Schema + '].'
		+ '[' + @Table + ']'
		
	EXEC (@SQL)

GO
