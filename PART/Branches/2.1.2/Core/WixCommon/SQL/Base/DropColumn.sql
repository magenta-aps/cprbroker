IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DropColumn]') AND type in (N'P', N'PC'))
	DROP PROCEDURE [dbo].[DropColumn]
GO

CREATE PROCEDURE DropColumn (
	@TableName sysname,
	@ColumnName sysname	
)
AS
	IF EXISTS (SELECT * FROM sys.columns c inner join sys.tables t on t.object_id=c.object_id  where c.name=@ColumnName and t.name=@TableName)
	BEGIN
		DECLARE @SQL VARCHAR(MAX) 
		SET @SQL = ''
			+ 'ALTER TABLE ' 
			+ '[' + @TableName + '] '
			+ 'DROP COLUMN '
			+ '[' + @ColumnName + '] ' 			

		IF NOT EXISTS (SELECT * FROM _InstallerSql WHERE SQL=@SQL)
			INSERT INTO _InstallerSql(SQL) VALUES (@SQL)
	END
		
GO 
