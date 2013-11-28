ALTER TABLE DataProviderCall ALTER COLUMN Success bit NULL

/****** Object:  Table [dbo].[BudgetInterval]    Script Date: 11/28/2013 16:33:20 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[BudgetInterval](
	[IntervalMillisecods] [bigint] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[CallThreshold] [int] NULL,
	[CostThreshold] [decimal](18, 4) NULL,
	[LastChecked] [datetime] NULL,
 CONSTRAINT [PK_BudgetInterval] PRIMARY KEY CLUSTERED 
(
	[IntervalMillisecods] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING ON
GO

