IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AddPrimaryKey]') AND type in (N'P', N'PC'))
	DROP PROCEDURE [dbo].AddPrimaryKey
GO

CREATE PROCEDURE AddPrimaryKey (
	@TableName sysname,
	@PKColumnName sysname,
	@PKColumnName2 sysname = NULL	
)
AS

	DECLARE @PkName VARCHAR(MAX)
	SET @PkName = 'PK_' + @TableName 
	
	IF NOT EXISTS (SELECT * FROM sys.key_constraints where name=@PkName)
	BEGIN
		DECLARE @SQL VARCHAR(MAX) 		
		
		
		SET @SQL = ''
			+ 'ALTER TABLE '
			+ '[' + @TableName + '] ' 
			+ 'ADD CONSTRAINT '
			+ '[' + @PkName + '] PRIMARY KEY CLUSTERED '
			+ '('
			+	'[' + @PKColumnName + ']'

		IF @PKColumnName2 IS NOT NULL
			SET @SQL = @SQL
				+ ', '
				+	'[' + @PKColumnName2 + ']'

		SET @SQL = @SQL
			+ ') WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]'			
		
		IF NOT EXISTS (SELECT * FROM _InstallerSql WHERE SQL=@SQL)
			INSERT INTO _InstallerSQL(SQL) VALUES (@SQL)		
		
	END
		
GO 