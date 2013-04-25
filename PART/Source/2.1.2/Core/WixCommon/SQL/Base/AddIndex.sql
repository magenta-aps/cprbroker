IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AddIndex]') AND type in (N'P', N'PC'))
	DROP PROCEDURE [dbo].[AddIndex]
GO

CREATE PROCEDURE AddIndex(
	@TableName sysname,
	@ColumnName sysname,
	@ColumnName2 sysname = NULL,
	@Unique BIT = 0
)
AS
	DECLARE @IndexName VARCHAR(MAX)
	SET @IndexName = 'IX_' + @TableName + '_' + @ColumnName 
	IF NOT EXISTS (SELECT * FROM sys.indexes where name=@IndexName)
	BEGIN
		DECLARE @SQL VARCHAR(MAX) 
		SET @SQL = ''
			+ 'CREATE ' 
			+ CASE(@Unique) WHEN 1 THEN 'UNIQUE ' ELSE '' END 
			+ 'NONCLUSTERED INDEX '
			+ '[' + @IndexName  + '] '
			+ 'ON '
			+ '[' + @TableName + '] '
			+ '('
			+ '['+ @ColumnName+ '] ' 
		
		IF @ColumnName2 IS NOT NULL 
			SET @SQL = @SQL
				+ ', [' + @ColumnName2 + ']'
		
		SET @SQL = @SQL
			+ ') '

		IF NOT EXISTS (SELECT * FROM _InstallerSql WHERE SQL=@SQL)
			INSERT INTO _InstallerSql(SQL) VALUES (@SQL)
	END
		
GO 
