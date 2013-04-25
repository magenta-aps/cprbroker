IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DropTable]') AND type in (N'P', N'PC'))
	DROP PROCEDURE [dbo].DropTable
GO

CREATE PROCEDURE DropTable (
	@TableName sysname
)
AS

	IF EXISTS (SELECT * FROM sys.tables where name=@TableName)
	BEGIN
		DECLARE @SQL VARCHAR(MAX) 
		SET @SQL = ''
			+ 'DROP TABLE '
			+ '[' + @TableName + '] '			
			
		IF NOT EXISTS (SELECT * FROM _InstallerSql WHERE SQL=@SQL)
			INSERT INTO _InstallerSQL(SQL) VALUES (@SQL)
	END
		
GO 