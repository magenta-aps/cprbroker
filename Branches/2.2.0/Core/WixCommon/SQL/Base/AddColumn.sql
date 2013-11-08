IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AddColumn]') AND type in (N'P', N'PC'))
	DROP PROCEDURE [dbo].[AddColumn]
GO

CREATE PROCEDURE AddColumn (
	@TableName sysname,
	@ColumnName sysname,
	@ColumnType varchar(100),
	@NotNull bit = 0
)
AS
	IF NOT EXISTS (SELECT * FROM sys.columns c inner join sys.tables t on t.object_id=c.object_id  where c.name=@ColumnName and t.name=@TableName)
	BEGIN
		DECLARE @SQL VARCHAR(MAX) 
		SET @SQL = ''
			+ 'ALTER TABLE ' 
			+ '[' + @TableName + '] '
			+ 'ADD '
			+ '[' + @ColumnName + '] ' 
			+ @ColumnType
			+ CASE(@NotNull) WHEN 0 THEN '' ELSE ' NOT NULL ' END 

		IF NOT EXISTS (SELECT * FROM _InstallerSql WHERE SQL=@SQL)
			INSERT INTO _InstallerSql(SQL) VALUES (@SQL)
	END
		
GO 
