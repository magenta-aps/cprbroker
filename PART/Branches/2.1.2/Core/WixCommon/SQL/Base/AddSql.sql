IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AddSql]') AND type in (N'P', N'PC'))
	DROP PROCEDURE [dbo].AddSql
GO

CREATE PROCEDURE AddSql (
	@SQL VARCHAR(MAX)
)
AS
	INSERT INTO _InstallerSQL(SQL) VALUES (@SQL)

GO 