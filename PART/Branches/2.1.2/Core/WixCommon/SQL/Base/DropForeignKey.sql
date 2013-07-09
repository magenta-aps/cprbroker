IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DropForeignKey]') AND type in (N'P', N'PC'))
	DROP PROCEDURE [dbo].[DropForeignKey]
GO

CREATE PROCEDURE DropForeignKey (
	@ChildTableName sysname,
	@ChildColumnName sysname,
	@ParentTableName sysname,
	@ParentColumnName sysname
)
AS
	DECLARE @ForeignKeyName VARCHAR(MAX)
	SET @ForeignKeyName = 'FK_' + @ChildTableName + '_' + @ParentTableName 
	IF EXISTS (SELECT * FROM sys.foreign_keys where name=@ForeignKeyName )
	BEGIN
		DECLARE @SQL VARCHAR(MAX) 
		SET @SQL = ''
			+ 'ALTER TABLE ' 
			+ '[' + @ChildTableName + '] '
			+ 'DROP CONSTRAINT '
			+ '['+ @ForeignKeyName + '] ' 
			
		IF NOT EXISTS (SELECT * FROM _InstallerSql WHERE SQL=@SQL)
			INSERT INTO _InstallerSql(SQL) VALUES (@SQL)
	END
		
GO 