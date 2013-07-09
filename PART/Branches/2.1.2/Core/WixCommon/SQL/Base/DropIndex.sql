IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DropIndex]') AND type in (N'P', N'PC'))
	DROP PROCEDURE [dbo].[DropIndex]
GO

CREATE PROCEDURE DropIndex(
	@TableName sysname,
	@ColumnName sysname,
	@Unique BIT = 0
	
)
AS
	DECLARE @IndexName VARCHAR(MAX)
	SET @IndexName = 'IX_' + @TableName + '_' + @ColumnName 
	IF EXISTS (SELECT * FROM sys.indexes where name=@IndexName)
	BEGIN
		DECLARE @SQL VARCHAR(MAX) 
		SET @SQL = ''
			+ 'DROP ' 			
			+ 'INDEX '
			+ '[' + @IndexName + + '] ' 
			+ 'ON '
			+ '[' + @TableName + '] '
			
			
		IF NOT EXISTS (SELECT * FROM _InstallerSql WHERE SQL=@SQL)
			INSERT INTO _InstallerSql(SQL) VALUES (@SQL)
	END
		
GO 
