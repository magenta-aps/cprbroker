-- Create table DataProviderCall to keep track of calls made to dataproviders
/****** Object:  Table [dbo].[DataProviderCall]    Script Date: 11/12/2013 14:07:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DataProviderCall]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DataProviderCall](
	[DataProviderCallId] [uniqueidentifier] NOT NULL,
	[ActivityId] [uniqueidentifier] NOT NULL,
	[CallTime] [datetime] NOT NULL,
	[DataProviderType] [varchar](250) NOT NULL,
	[Cost] [decimal](18, 4) NOT NULL,
	[Operation] [varchar](250) NOT NULL,
	[Input] [varchar](250) NOT NULL,
	[Success] [bit] NOT NULL,
 CONSTRAINT [PK_DataProviderCall] PRIMARY KEY NONCLUSTERED 
(
	[DataProviderCallId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING ON
GO
/****** Object:  Default [DF__DataProviderCall_DataProviderCallId]    Script Date: 11/12/2013 14:07:02 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF__DataProviderCall_DataProviderCallId]') AND parent_object_id = OBJECT_ID(N'[dbo].[DataProviderCall]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__DataProviderCall_DataProviderCallId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[DataProviderCall] ADD  CONSTRAINT [DF__DataProviderCall_DataProviderCallId]  DEFAULT (newid()) FOR [DataProviderCallId]
END


End
GO
/****** Object:  Default [DF_DataProviderCall_CallTime]    Script Date: 11/12/2013 14:07:02 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_DataProviderCall_CallTime]') AND parent_object_id = OBJECT_ID(N'[dbo].[DataProviderCall]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_DataProviderCall_CallTime]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[DataProviderCall] ADD  CONSTRAINT [DF_DataProviderCall_CallTime]  DEFAULT (getdate()) FOR [CallTime]
END


End
GO


-- Add the field ActivityID to LogEntry in order to link it to rows in DataProviderCall
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('LogEntry') and name = 'ActivityId')
Begin
	alter table LogEntry add ActivityId uniqueidentifier
End


----------------------------------------
-- Now delete the unused child tables --
----------------------------------------

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'HealthInformation'))
    DROP TABLE HealthInformation
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'UnknownCitizenData'))
    DROP TABLE UnknownCitizenData
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'PersonRelationship'))
    DROP TABLE PersonRelationship
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'GreenlandicAddress'))
    DROP TABLE GreenlandicAddress
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'PersonCivilState'))
    DROP TABLE PersonCivilState
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'PersonLifeState'))
    DROP TABLE PersonLifeState
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'CprData'))
    DROP TABLE CprData
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'ForeignAddress'))
    DROP TABLE ForeignAddress
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'PersonName'))
    DROP TABLE PersonName
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'PersonProperties'))
    DROP TABLE PersonProperties
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'ContactChannel'))
    DROP TABLE ContactChannel
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'ContactChannelType'))
    DROP TABLE ContactChannelType
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'AddressPointStatus'))
    DROP TABLE AddressPointStatus
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'ForeignCitizenCountry'))
    DROP TABLE ForeignCitizenCountry
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'ForeignCitizenData'))
    DROP TABLE ForeignCitizenData
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'Gender'))
    DROP TABLE Gender
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'GeographicCoorditaneTuple'))
    DROP TABLE GeographicCoorditaneTuple
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'GeographicPointLocation'))
    DROP TABLE GeographicPointLocation
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'AddressPoint'))
    DROP TABLE AddressPoint
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'DanishAddress'))
    DROP TABLE DanishAddress
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'PersonAttributes'))
    DROP TABLE PersonAttributes
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'RelationshipType'))
    DROP TABLE RelationshipType
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'LifeStatusCodeType'))
    DROP TABLE LifeStatusCodeType
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'Effect'))
    DROP TABLE Effect
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'PersonState'))
    DROP TABLE PersonState
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'CivilStatusCodeType'))
    DROP TABLE CivilStatusCodeType
GO