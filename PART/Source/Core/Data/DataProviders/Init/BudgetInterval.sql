IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BudgetInterval]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[BudgetInterval](
	    [IntervalMilliseconds] [bigint] NOT NULL
            CONSTRAINT [PK_BudgetInterval] PRIMARY KEY CLUSTERED ([IntervalMilliseconds] ASC),
	    [Name] [varchar](50) NOT NULL,
	    [CallThreshold] [int] NULL,
	    [CostThreshold] [decimal](18, 4) NULL,
	    [LastChecked] [datetime] NULL
    ) ON [PRIMARY]
END

GO
