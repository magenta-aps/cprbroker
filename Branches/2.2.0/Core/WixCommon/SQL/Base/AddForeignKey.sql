IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AddForeignKey]') AND type in (N'P', N'PC'))
	DROP PROCEDURE [dbo].[AddForeignKey]
GO

CREATE PROCEDURE AddForeignKey (
	@ChildTableName sysname,
	@ChildColumnName sysname,
	@ParentTableName sysname,
	@ParentColumnName sysname,
	@Cascade BIT = 0
)
AS
	DECLARE @ForeignKeyName VARCHAR(MAX)
	SET @ForeignKeyName = 'FK_' + @ChildTableName + '_' + @ParentTableName 
	IF NOT EXISTS (SELECT * FROM sys.foreign_keys where name=@ForeignKeyName )
	BEGIN
		DECLARE @SQL VARCHAR(MAX) 
		SET @SQL = ''
			+ 'ALTER TABLE ' 
			+ '[' + @ChildTableName + '] '
			+ 'ADD CONSTRAINT '
			+ '['+ @ForeignKeyName + '] ' 
			+ 'FOREIGN KEY '
			+ '('
			+ '['+ @ChildColumnName+ ']'
			+ ') '
			+ 'REFERENCES '
			+ '['+ @ParentTableName+ ']'
			+ '('
			+ '['+ @ParentColumnName+ ']'
			+ ') '
			+ 'ON UPDATE ' + CASE (@Cascade) WHEN 1 THEN 'CASCADE' ELSE 'NO ACTION' END + ' '
			+ 'ON DELETE ' + CASE (@Cascade) WHEN 1 THEN 'CASCADE' ELSE 'NO ACTION' END + ' '			
			
		IF NOT EXISTS (SELECT * FROM _InstallerSql WHERE SQL=@SQL)
			INSERT INTO _InstallerSql(SQL) VALUES (@SQL)
	END
		
GO 
