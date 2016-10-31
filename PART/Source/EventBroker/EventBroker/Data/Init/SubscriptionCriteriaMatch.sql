IF EXISTS (SELECT * FROM sys.tables WHERE object_id = object_id('SubscriptionCriteriaMatch'))
    DROP TABLE [dbo].[SubscriptionCriteriaMatch]

-------------------------------------
-- Table : SubscriptionCriteriaMatch
-------------------------------------

IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id = object_id('SubscriptionCriteriaMatch'))
BEGIN
	CREATE TABLE [dbo].[SubscriptionCriteriaMatch](
		[SubscriptionCriteriaMatchId] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
		[SubscriptionId] [uniqueidentifier] NOT NULL,
		[DataChangeEventId] [uniqueidentifier] NOT NULL,
		CONSTRAINT [PK_SubscriptionCriteriaMatch] PRIMARY KEY CLUSTERED ( [SubscriptionCriteriaMatchId] ASC) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
		CONSTRAINT [FK_SubscriptionCriteriaMatch_DataChangeEvent] FOREIGN KEY([DataChangeEventId]) REFERENCES [dbo].[DataChangeEvent] ([DataChangeEventId]) ON UPDATE CASCADE ON DELETE CASCADE,
		CONSTRAINT [FK_SubscriptionCriteriaMatch_Subscription] FOREIGN KEY([SubscriptionId]) REFERENCES [dbo].[Subscription] ([SubscriptionId]) ON UPDATE CASCADE ON DELETE CASCADE
	) ON [PRIMARY]
END

GO