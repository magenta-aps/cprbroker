IF  EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[dbo].[_InstallerSql]') AND type in (N'U'))
	DROP TABLE [dbo]._InstallerSql
GO

CREATE TABLE _InstallerSql(
	ID INT IDENTITY,
	SQL VARCHAR(MAX),
	ExecutionDate DATETIME
)
		
GO 
