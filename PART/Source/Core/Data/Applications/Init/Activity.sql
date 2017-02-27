IF NOT EXISTS (SELECT * FROM sys.tables WHERE name=N'Activity')
BEGIN
	CREATE TABLE Activity(
		-- Data columns
		ActivityId	UNIQUEIDENTIFIER			CONSTRAINT DF_Activity_ActivityId DEFAULT NEWID(),
		ApplicationId UNIQUEIDENTIFIER NOT NULL CONSTRAINT FK_Activity_Application REFERENCES [Application](ApplicationId),
		StartTS		DATETIME NOT NULL			CONSTRAINT DF_Activity_StartTS	  DEFAULT GETDATE(),
		UserToken	VARCHAR(250) NULL,
		UserId		VARCHAR(250) NULL,
		MethodName	VARCHAR(250) NULL,

		-- Statistics columns
		HasCriticalErrors BIT CONSTRAINT DF_Activity_HasCriticalErrors DEFAULT 0,
		HasErrors BIT CONSTRAINT DF_Activity_HasErrors DEFAULT 0,
		HasWarnings BIT CONSTRAINT DF_Activity_HasWarnings DEFAULT 0,
		HasInformation BIT CONSTRAINT DF_Activity_HasInformation DEFAULT 0,
		HasDataProviderCalls BIT CONSTRAINT DF_Activity_HasDataProviderCalls DEFAULT 0,
		HasOperations BIT CONSTRAINT DF_Activity_HasOperations DEFAULT 0

		-- Primary key
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
