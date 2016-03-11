IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Semaphore') AND name = 'WaitCount' )
ALTER TABLE [dbo].[Semaphore]
    ADD [WaitCount] INT DEFAULT 1 NULL;