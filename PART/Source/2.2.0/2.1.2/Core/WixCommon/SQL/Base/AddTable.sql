IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AddTable]') AND type in (N'P', N'PC'))
	DROP PROCEDURE [dbo].AddTable
GO

CREATE PROCEDURE AddTable (
	@TableName sysname,
	@PKColumnName sysname,
	@PKColumnType VARCHAR(100)	
)
AS

	IF NOT EXISTS (SELECT * FROM sys.tables where name=@TableName)
	BEGIN
		DECLARE @SQL VARCHAR(MAX) 
		SET @SQL = ''
			+ 'CREATE TABLE '
			+ '[' + @TableName + '] '
			+ '( '
			+ '[' + @PKColumnName + '] ' 
			+ @PKColumnType 
			+ ' NOT NULL ' 	
			+ ')'			
			
		IF NOT EXISTS (SELECT * FROM _InstallerSql WHERE SQL=@SQL)
			INSERT INTO _InstallerSQL(SQL) VALUES (@SQL)	
		
	END
		
GO 