IF NOT EXISTS (SELECT * FROM sys.tables WHERE name=N'Activity')
BEGIN
	CREATE TABLE Activity(
		ActivityId	UNIQUEIDENTIFIER	CONSTRAINT DF_Activity_ActivityId DEFAULT NEWID(),
		ApplicationId UNIQUEIDENTIFIER  CONSTRAINT FK_Activity_Application REFERENCES [Application](ApplicationId),
		StartTS		DATETIME			CONSTRAINT DF_Activity_StartTS	  DEFAULT GETDATE(),
		UserToken	VARCHAR(250) NULL,
		UserId		VARCHAR(250) NULL,
		MethodName	VARCHAR(250) NULL,

		CONSTRAINT PK_Activity PRIMARY KEY NONCLUSTERED (ActivityId),		

	)ON [PRIMARY] 
END
GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Activity]') AND name = N'IX_Activity_StartTS')
BEGIN
	CREATE CLUSTERED INDEX IX_Activity_StartTS ON Activity(
		StartTS ASC
	)
END
GO
