/*
    =========================
    Table: PersonRegistration
    =========================
*/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PersonRegistration]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PersonRegistration](
        [PersonRegistrationId] [uniqueidentifier] NOT NULL,
        [UUID] [uniqueidentifier] NOT NULL,
        [ActorRefId] [uniqueidentifier] NULL,
        [RegistrationDate] [datetime] NOT NULL,
        [BrokerUpdateDate] [datetime] NOT NULL,
        [CommentText] [varchar](50) NULL,
        [LifecycleStatusId] [int] NOT NULL,
        [Contents] [xml] NULL,
        [SourceObjects] [xml] NULL,
    
        CONSTRAINT [PK_Registration] PRIMARY KEY NONCLUSTERED 
        ([PersonRegistrationId] ASC) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
    ) ON [PRIMARY]
END
GO

/*
    ===================================================
    Clustered index on UUID: IX_PersonRegistration_UUID
    ===================================================
*/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PersonRegistration]') AND name = N'IX_PersonRegistration_UUID')
    CREATE CLUSTERED INDEX [IX_PersonRegistration_UUID] ON [dbo].[PersonRegistration] 
    ([UUID] ASC) 
    WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

/*
    ==========================================
    DF_PersonRegistration_PersonRegistrationId
    ==========================================
*/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_PersonRegistration_PersonRegistrationId]') AND parent_object_id = OBJECT_ID(N'[dbo].[PersonRegistration]'))
BEGIN
    IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_PersonRegistration_PersonRegistrationId]') AND type = 'D')    
        ALTER TABLE [dbo].[PersonRegistration] ADD  CONSTRAINT [DF_PersonRegistration_PersonRegistrationId]  DEFAULT (newid()) FOR [PersonRegistrationId]    
END
GO

/*
    ==============================
    FK_PersonRegistration_ActorRef
    ==============================
*/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PersonRegistration_ActorRef]') AND parent_object_id = OBJECT_ID(N'[dbo].[PersonRegistration]'))
    ALTER TABLE [dbo].[PersonRegistration]  WITH CHECK ADD  CONSTRAINT [FK_PersonRegistration_ActorRef] FOREIGN KEY([ActorRefId])
    REFERENCES [dbo].[ActorRef] ([ActorRefId])
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PersonRegistration_ActorRef]') AND parent_object_id = OBJECT_ID(N'[dbo].[PersonRegistration]'))
ALTER TABLE [dbo].[PersonRegistration] CHECK CONSTRAINT [FK_PersonRegistration_ActorRef]
GO

/*
    =====================================
    FK_PersonRegistration_LifecycleStatus
    =====================================
*/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PersonRegistration_LifecycleStatus]') AND parent_object_id = OBJECT_ID(N'[dbo].[PersonRegistration]'))
    ALTER TABLE [dbo].[PersonRegistration]  WITH CHECK ADD  CONSTRAINT [FK_PersonRegistration_LifecycleStatus] FOREIGN KEY([LifecycleStatusId])
    REFERENCES [dbo].[LifecycleStatus] ([LifecycleStatusId])
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PersonRegistration_LifecycleStatus]') AND parent_object_id = OBJECT_ID(N'[dbo].[PersonRegistration]'))
    ALTER TABLE [dbo].[PersonRegistration] CHECK CONSTRAINT [FK_PersonRegistration_LifecycleStatus]
GO

/*
    ============================
    FK_PersonRegistration_Person
    ============================
*/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PersonRegistration_Person]') AND parent_object_id = OBJECT_ID(N'[dbo].[PersonRegistration]'))
    ALTER TABLE [dbo].[PersonRegistration]  WITH CHECK ADD  CONSTRAINT [FK_PersonRegistration_Person] FOREIGN KEY([UUID])
    REFERENCES [dbo].[Person] ([UUID])
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PersonRegistration_Person]') AND parent_object_id = OBJECT_ID(N'[dbo].[PersonRegistration]'))
    ALTER TABLE [dbo].[PersonRegistration] CHECK CONSTRAINT [FK_PersonRegistration_Person]
GO
