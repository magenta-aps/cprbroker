IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'Activity') AND name = N'HasCriticalErrors')
	ALTER TABLE Activity ADD
		HasCriticalErrors BIT CONSTRAINT DF_Activity_HasCriticalErrors DEFAULT 0,
		HasErrors BIT CONSTRAINT DF_Activity_HasErrors DEFAULT 0,
		HasWarnings BIT CONSTRAINT DF_Activity_HasWarnings DEFAULT 0,
		HasInformation BIT CONSTRAINT DF_Activity_HasInformation DEFAULT 0,
		HasDataProviderCalls BIT CONSTRAINT DF_Activity_HasDataProviderCalls DEFAULT 0,
		HasOperations BIT CONSTRAINT DF_Activity_HasOperations DEFAULT 0