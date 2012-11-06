/*==============================================================*/
/* DBMS name:      Microsoft SQL Server 2008                    */
/* Created on:     10-03-2011 11:42:08                          */
/*==============================================================*/


if exists (select 1
          from sysobjects
          where  id = object_id('fnGK_CORE_DatetimeMaxValue')
          and type in ('IF', 'FN', 'TF'))
   drop function fnGK_CORE_DatetimeMaxValue
go

if exists (select 1
          from sysobjects
          where  id = object_id('fnGK_CORE_DatetimeMinValue')
          and type in ('IF', 'FN', 'TF'))
   drop function fnGK_CORE_DatetimeMinValue
go

if exists (select 1
          from sysobjects
          where  id = object_id('fnGK_CORE_GetConfigValue')
          and type in ('IF', 'FN', 'TF'))
   drop function fnGK_CORE_GetConfigValue
go

if exists (select 1
          from sysobjects
          where  id = object_id('fnGK_CORE_GetStringFromGUID')
          and type in ('IF', 'FN', 'TF'))
   drop function fnGK_CORE_GetStringFromGUID
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_PMU_SetCPRLoginNameMap')
          and type in ('P','PC'))
   drop procedure spGK_PMU_SetCPRLoginNameMap
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_PMU_GetLoginNameFromCPR')
          and type in ('P','PC'))
   drop procedure spGK_PMU_GetLoginNameFromCPR
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_PMU_GetCPRFromLoginName')
          and type in ('P','PC'))
   drop procedure spGK_PMU_GetCPRFromLoginName
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_PMU_RenameLoginName')
          and type in ('P','PC'))
   drop procedure spGK_PMU_RenameLoginName
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_PMU_DeleteLoginName')
          and type in ('P','PC'))
   drop procedure spGK_PMU_DeleteLoginName
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_PMU_LoginNameExist')
          and type in ('P','PC'))
   drop procedure spGK_PMU_LoginNameExist
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_PMU_SetPreferredUA')
          and type in ('P','PC'))
   drop procedure spGK_PMU_SetPreferredUA
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_PMU_GetCurrentLoginRecord')
          and type in ('P','PC'))
   drop procedure spGK_PMU_GetCurrentLoginRecord
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_PMU_VerifyPreferredUA')
          and type in ('P','PC'))
   drop procedure spGK_PMU_VerifyPreferredUA
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_N2L_RegisterNonAdminUser')
          and type in ('P','PC'))
   drop procedure spGK_N2L_RegisterNonAdminUser
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_N2L_IsRegisteredNonAdminUser')
          and type in ('P','PC'))
   drop procedure spGK_N2L_IsRegisteredNonAdminUser
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_N2L_IsRegisteredExplicitAccess')
          and type in ('P','PC'))
   drop procedure spGK_N2L_IsRegisteredExplicitAccess
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_N2L_RegisterExplicitAccess')
          and type in ('P','PC'))
   drop procedure spGK_N2L_RegisterExplicitAccess
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_PM_GetObjectIDFromCPR')
          and type in ('P','PC'))
   drop procedure spGK_PM_GetObjectIDFromCPR
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_PM_GetObjectIDsFromCPRArray')
          and type in ('P','PC'))
   drop procedure spGK_PM_GetObjectIDsFromCPRArray
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_PM_GetCPRFromObjectID')
          and type in ('P','PC'))
   drop procedure spGK_PM_GetCPRFromObjectID
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_PM_GetCPRsFromObjectIDArray')
          and type in ('P','PC'))
   drop procedure spGK_PM_GetCPRsFromObjectIDArray

GO

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_PM_InitDB')
          and type in ('P','PC'))
   drop procedure spGK_PM_InitDB
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_PM_GetOwnerIDFromNamespace')
          and type in ('P','PC'))
   drop procedure spGK_PM_GetOwnerIDFromNamespace
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_CORE_GetRuntimeInfo')
          and type in ('P','PC'))
   drop procedure spGK_CORE_GetRuntimeInfo
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_CORE_TryCatchSnippet01')
          and type in ('P','PC'))
   drop procedure spGK_CORE_TryCatchSnippet01
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_N2L_GetDeploymentPeriodStartStop')
          and type in ('P','PC'))
   drop procedure spGK_N2L_GetDeploymentPeriodStartStop
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_CORE_ErrorHandler')
          and type in ('P','PC'))
   drop procedure spGK_CORE_ErrorHandler
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_PMU_PrepareSPInvocation')
          and type in ('P','PC'))
   drop procedure spGK_PMU_PrepareSPInvocation
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_PM_PrepareSPInvocation')
          and type in ('P','PC'))
   drop procedure spGK_PM_PrepareSPInvocation
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_CORE_GetCorrIDFromContext')
          and type in ('P','PC'))
   drop procedure spGK_CORE_GetCorrIDFromContext
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_CORE_LogAuditEntry')
          and type in ('P','PC'))
   drop procedure spGK_CORE_LogAuditEntry
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_CORE_LogAuditEntryExt')
          and type in ('P','PC'))
   drop procedure spGK_CORE_LogAuditEntryExt
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_CORE_LogErrorEntry')
          and type in ('P','PC'))
   drop procedure spGK_CORE_LogErrorEntry
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_CORE_LogEntry')
          and type in ('P','PC'))
   drop procedure spGK_CORE_LogEntry
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_CORE_SetConfigValue')
          and type in ('P','PC'))
   drop procedure spGK_CORE_SetConfigValue
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_CORE_ValidateCPR')
          and type in ('P','PC'))
   drop procedure spGK_CORE_ValidateCPR
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_PMU_InsertDefaultPreferred')
          and type in ('P','PC'))
   drop procedure spGK_PMU_InsertDefaultPreferred
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_PM_Probe')
          and type in ('P','PC'))
   drop procedure spGK_PM_Probe
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('T_CORE_Log') and o.name = 'FK_CORE_LogType_From_CORE_Log')
alter table T_CORE_Log
   drop constraint FK_CORE_LogType_From_CORE_Log
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('T_CORE_Log') and o.name = 'FK_CORE_Package_From_CORE_Log')
alter table T_CORE_Log
   drop constraint FK_CORE_Package_From_CORE_Log
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('T_N2L_AccessOverride') and o.name = 'FK_PersonMaster_From_AccessOverride')
alter table T_N2L_AccessOverride
   drop constraint FK_PersonMaster_From_AccessOverride
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('T_N2L_NonAdminUser') and o.name = 'FK_PersonMaster_From_NonAdminUser')
alter table T_N2L_NonAdminUser
   drop constraint FK_PersonMaster_From_NonAdminUser
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('T_PMU_UserAccount') and o.name = 'FK_PersonMaster_From_UserAccount')
alter table T_PMU_UserAccount
   drop constraint FK_PersonMaster_From_UserAccount
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('T_PMU_UserAccount_Preferred') and o.name = 'FK_PersonMaster_From_UserAccount_Preferred')
alter table T_PMU_UserAccount_Preferred
   drop constraint FK_PersonMaster_From_UserAccount_Preferred
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('T_PM_CPR') and o.name = 'FK_Gender_From_CPR')
alter table T_PM_CPR
   drop constraint FK_Gender_From_CPR
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('T_PM_CPR') and o.name = 'FK_PersonMaster_From_CPR')
alter table T_PM_CPR
   drop constraint FK_PersonMaster_From_CPR
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('T_PM_PersonMaster') and o.name = 'FK_ObjectOwner_From_PersonMaster')
alter table T_PM_PersonMaster
   drop constraint FK_ObjectOwner_From_PersonMaster
go

if exists (select 1
            from  sysobjects
           where  id = object_id('T_CORE_Config')
            and   type = 'U')
   drop table T_CORE_Config
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('T_CORE_Log')
            and   name  = 'T_CORE_Log_corrID_IDX'
            and   indid > 0
            and   indid < 255)
   drop index T_CORE_Log.T_CORE_Log_corrID_IDX
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('T_CORE_Log')
            and   name  = 'T_CORE_Log_Package_IDX'
            and   indid > 0
            and   indid < 255)
   drop index T_CORE_Log.T_CORE_Log_Package_IDX
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('T_CORE_Log')
            and   name  = 'T_CORE_Log_LogType_IDX'
            and   indid > 0
            and   indid < 255)
   drop index T_CORE_Log.T_CORE_Log_LogType_IDX
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('T_CORE_Log')
            and   name  = 'T_CORE_Log_LogTime_IDX'
            and   indid > 0
            and   indid < 255)
   drop index T_CORE_Log.T_CORE_Log_LogTime_IDX
go

if exists (select 1
            from  sysobjects
           where  id = object_id('T_CORE_Log')
            and   type = 'U')
   drop table T_CORE_Log
go

if exists (select 1
            from  sysobjects
           where  id = object_id('T_CORE_LogType')
            and   type = 'U')
   drop table T_CORE_LogType
go

if exists (select 1
            from  sysobjects
           where  id = object_id('T_CORE_Package')
            and   type = 'U')
   drop table T_CORE_Package
go

if exists (select 1
            from  sysobjects
           where  id = object_id('T_N2L_AccessOverride')
            and   type = 'U')
   drop table T_N2L_AccessOverride
go

if exists (select 1
            from  sysobjects
           where  id = object_id('T_N2L_NonAdminUser')
            and   type = 'U')
   drop table T_N2L_NonAdminUser
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('T_PMU_UserAccount')
            and   name  = 'T_UserCredential_IDX_LOGINNAME'
            and   indid > 0
            and   indid < 255)
   drop index T_PMU_UserAccount.T_UserCredential_IDX_LOGINNAME
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('T_PMU_UserAccount')
            and   name  = 'T_UserCredential_IDX_PERSONMASTER'
            and   indid > 0
            and   indid < 255)
   drop index T_PMU_UserAccount.T_UserCredential_IDX_PERSONMASTER
go

if exists (select 1
            from  sysobjects
           where  id = object_id('T_PMU_UserAccount')
            and   type = 'U')
   drop table T_PMU_UserAccount
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('T_PMU_UserAccount_Preferred')
            and   name  = 'T_UA_Preferred_IDX_PERSONMASTER'
            and   indid > 0
            and   indid < 255)
   drop index T_PMU_UserAccount_Preferred.T_UA_Preferred_IDX_PERSONMASTER
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('T_PMU_UserAccount_Preferred')
            and   name  = 'T_UA_Preferred_IDX_UserAccountID'
            and   indid > 0
            and   indid < 255)
   drop index T_PMU_UserAccount_Preferred.T_UA_Preferred_IDX_UserAccountID
go

if exists (select 1
            from  sysobjects
           where  id = object_id('T_PMU_UserAccount_Preferred')
            and   type = 'U')
   drop table T_PMU_UserAccount_Preferred
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('T_PM_CPR')
            and   name  = 'T_CPR_IDX_PERSONMASTER'
            and   indid > 0
            and   indid < 255)
   drop index T_PM_CPR.T_CPR_IDX_PERSONMASTER
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('T_PM_CPR')
            and   name  = 'T_CPR_IDX_BIRTHDATE'
            and   indid > 0
            and   indid < 255)
   drop index T_PM_CPR.T_CPR_IDX_BIRTHDATE
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('T_PM_CPR')
            and   name  = 'T_CPR_IDX_CPRNO'
            and   indid > 0
            and   indid < 255)
   drop index T_PM_CPR.T_CPR_IDX_BIRTHDATE
go

if exists (select 1
            from  sysobjects
           where  id = object_id('T_PM_CPR')
            and   type = 'U')
   drop table T_PM_CPR
go

if exists (select 1
            from  sysobjects
           where  id = object_id('T_PM_Gender')
            and   type = 'U')
   drop table T_PM_Gender
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('T_PM_ObjectOwner')
            and   name  = 'T_ObjectOwner_IDX_Namespace'
            and   indid > 0
            and   indid < 255)
   drop index T_PM_ObjectOwner.T_ObjectOwner_IDX_Namespace
go

if exists (select 1
            from  sysobjects
           where  id = object_id('T_PM_ObjectOwner')
            and   type = 'U')
   drop table T_PM_ObjectOwner
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('T_PM_PersonMaster')
            and   name  = 'T_PersonMaster_IDX_ObjectOwner'
            and   indid > 0
            and   indid < 255)
   drop index T_PM_PersonMaster.T_PersonMaster_IDX_ObjectOwner
go

if exists (select 1
            from  sysobjects
           where  id = object_id('T_PM_PersonMaster')
            and   type = 'U')
   drop table T_PM_PersonMaster
go

/*==============================================================*/
/* Table: T_CORE_Config                                         */
/*==============================================================*/
create table T_CORE_Config (
   name                 varchar(60)          not null,
   value                varchar(1020)        not null,
   constraint PK_T_CORE_CONFIG primary key (name)
)
go

INSERT INTO T_CORE_Config VALUES ('namespace-self', '<pm-namespace>')
INSERT INTO T_CORE_Config VALUES ('db-version', '1.2')
INSERT INTO T_CORE_Config VALUES ('db-environment', 'PROD')
INSERT INTO T_CORE_Config VALUES ('loglevel', '4')

/*==============================================================*/
/* Table: T_CORE_Log                                            */
/*==============================================================*/
create table T_CORE_Log (
   ID                   int                  identity,
   logTime              datetime             not null,
   logType              int                  not null,
   package              int                  not null,
   context              varchar(1020)        not null,
   corrID               varchar(60)          not null,
   spName               varchar(120)         not null,
   eventID              integer              not null,
   logText              varchar(2000)        not null,
   constraint PK_T_CORE_LOG primary key (ID)
)
go

/*==============================================================*/
/* Index: T_CORE_Log_LogTime_IDX                                */
/*==============================================================*/
create index T_CORE_Log_LogTime_IDX on T_CORE_Log (
logTime ASC
)
go

/*==============================================================*/
/* Index: T_CORE_Log_LogType_IDX                                */
/*==============================================================*/
create index T_CORE_Log_LogType_IDX on T_CORE_Log (
logType ASC
)
go

/*==============================================================*/
/* Index: T_CORE_Log_Package_IDX                                */
/*==============================================================*/
create index T_CORE_Log_Package_IDX on T_CORE_Log (
package ASC
)
go

/*==============================================================*/
/* Index: T_CORE_Log_corrID_IDX                                 */
/*==============================================================*/
create index T_CORE_Log_corrID_IDX on T_CORE_Log (
corrID ASC
)
go

/*==============================================================*/
/* Table: T_CORE_LogType                                        */
/*==============================================================*/
create table T_CORE_LogType (
   typeID               int                  not null,
   typeName             varchar(15)          not null,
   constraint PK_T_CORE_LOGTYPE primary key (typeID)
)
go

INSERT INTO T_CORE_LogType VALUES (-1, 'UNDEFINED')
INSERT INTO T_CORE_LogType VALUES (0, 'RESERVED')
INSERT INTO T_CORE_LogType VALUES (1, 'AUDIT')
INSERT INTO T_CORE_LogType VALUES (2, 'ERROR')
INSERT INTO T_CORE_LogType VALUES (3, 'INFO')
INSERT INTO T_CORE_LogType VALUES (4, 'TRACE')
INSERT INTO T_CORE_LogType VALUES (5, 'DEBUG')

/*==============================================================*/
/* Table: T_CORE_Package                                        */
/*==============================================================*/
create table T_CORE_Package (
   packageID            int                  not null,
   prefix               varchar(15)          not null,
   description          varchar(250)         not null,
   constraint PK_T_CORE_PACKAGE primary key (packageID)
)
go

INSERT INTO T_CORE_Package VALUES (-1, 'UNDEF', 'Undefined')
INSERT INTO T_CORE_Package VALUES (1, 'CORE', 'Prefix for GK CORE DB and GkApp framework related DB objects')
INSERT INTO T_CORE_Package VALUES (2, 'PM', 'Prefix for PersonmasterService DB objects')
INSERT INTO T_CORE_Package VALUES (3, 'PMU', 'Prefix for login/user account handling DB objects')
INSERT INTO T_CORE_Package VALUES (4, 'N2L', 'Prefix for NemID2LAN DB objects')

/*==============================================================*/
/* Table: T_N2L_AccessOverride                                  */
/*==============================================================*/
create table T_N2L_AccessOverride (
   personMasterID       uniqueidentifier     not null,
   createTS             datetime             not null,
   accessAllowed        int                  not null,
   constraint PK_T_N2L_ACCESSOVERRIDE primary key (personMasterID)
)
go

/*==============================================================*/
/* Table: T_N2L_NonAdminUser                                    */
/*==============================================================*/
create table T_N2L_NonAdminUser (
   personMasterID       uniqueidentifier     not null,
   createTS             datetime             not null,
   constraint PK_T_N2L_NONADMINUSER primary key (personMasterID)
)
go

/*==============================================================*/
/* Table: T_PMU_UserAccount                                     */
/*==============================================================*/
create table T_PMU_UserAccount (
   objectID             uniqueidentifier     not null,
   loginName            varchar(30)          not null,
   personMasterID       uniqueidentifier     not null,
   transactFrom         datetime             not null,
   transactTo           datetime             not null,
   constraint PK_T_PMU_USERACCOUNT primary key (objectID, transactTo)
)
go

/*==============================================================*/
/* Index: T_UserCredential_IDX_PERSONMASTER                     */
/*==============================================================*/
create index T_UserCredential_IDX_PERSONMASTER on T_PMU_UserAccount (
personMasterID ASC
)
go

/*==============================================================*/
/* Index: T_UserCredential_IDX_LOGINNAME                        */
/*==============================================================*/
create unique index T_UserCredential_IDX_LOGINNAME on T_PMU_UserAccount (
loginName ASC,
transactTo ASC
)
go

/*==============================================================*/
/* Table: T_PMU_UserAccount_Preferred                           */
/*==============================================================*/
create table T_PMU_UserAccount_Preferred (
   personMasterID       uniqueidentifier     not null,
   uaObjectID           uniqueidentifier     not null,
   constraint PK_T_PMU_USERACCOUNT_PREFERRED primary key (personMasterID)
)
go

/*==============================================================*/
/* Index: T_UA_Preferred_IDX_UserAccountID                      */
/*==============================================================*/
create unique index T_UA_Preferred_IDX_UserAccountID on T_PMU_UserAccount_Preferred (
uaObjectID ASC
)
go

/*==============================================================*/
/* Index: T_UA_Preferred_IDX_PERSONMASTER                       */
/*==============================================================*/
create unique index T_UA_Preferred_IDX_PERSONMASTER on T_PMU_UserAccount_Preferred (
personMasterID ASC
)
go

/*==============================================================*/
/* Table: T_PM_CPR                                              */
/*==============================================================*/
create table T_PM_CPR (
   encryptedCprNo       varbinary(90)        not null,
   birthDate            datetime             not null,
   gender               int                  not null,
   personMasterID       uniqueidentifier     not null,
   createTS             datetime             not null,
   cprNo                varchar(10)          not null,
   constraint PK_T_PM_CPR primary key (encryptedCprNo)
)
go

/*==============================================================*/
/* Index: T_CPR_IDX_BIRTHDATE                                   */
/*==============================================================*/
create index T_CPR_IDX_BIRTHDATE on T_PM_CPR (
birthDate ASC
)
go

/*==============================================================*/
/* Index: T_CPR_IDX_PERSONMASTER                                */
/*==============================================================*/
create unique index T_CPR_IDX_PERSONMASTER on T_PM_CPR (
personMasterID ASC
)
go

/*==============================================================*/
/* Index: T_CPR_IDX_CPRNO                                */
/*==============================================================*/
CREATE NONCLUSTERED INDEX T_CPR_IDX_CPRNO ON dbo.T_PM_CPR
	(
	cprNo
	)
go


/*==============================================================*/
/* Table: T_PM_Gender                                           */
/*==============================================================*/
create table T_PM_Gender (
   ID                   int                  not null,
   description          varchar(250)         not null,
   constraint PK_T_PM_GENDER primary key (ID)
)
go

INSERT INTO T_PM_Gender VALUES (-1,'Unknown')
INSERT INTO T_PM_Gender VALUES (0,'Female')
INSERT INTO T_PM_Gender VALUES (1,'Male')

/*==============================================================*/
/* Table: T_PM_ObjectOwner                                      */
/*==============================================================*/
create table T_PM_ObjectOwner (
   objectOwnerID        uniqueidentifier     not null,
   ownerNamespace       varchar(510)         not null,
   createTS             datetime             not null,
   constraint PK_T_PM_OBJECTOWNER primary key (objectOwnerID)
)
go

/*==============================================================*/
/* Index: T_ObjectOwner_IDX_Namespace                           */
/*==============================================================*/
create unique index T_ObjectOwner_IDX_Namespace on T_PM_ObjectOwner (
ownerNamespace ASC
)
go

/*==============================================================*/
/* Table: T_PM_PersonMaster                                     */
/*==============================================================*/
create table T_PM_PersonMaster (
   objectID             uniqueidentifier     not null,
   objectOwnerID        uniqueidentifier     not null,
   createTS             datetime             not null,
   constraint PK_T_PM_PERSONMASTER primary key (objectID)
)
go

/*==============================================================*/
/* Index: T_PersonMaster_IDX_ObjectOwner                        */
/*==============================================================*/
create index T_PersonMaster_IDX_ObjectOwner on T_PM_PersonMaster (
objectOwnerID ASC
)
go

alter table T_CORE_Log
   add constraint FK_CORE_LogType_From_CORE_Log foreign key (logType)
      references T_CORE_LogType (typeID)
go

alter table T_CORE_Log
   add constraint FK_CORE_Package_From_CORE_Log foreign key (package)
      references T_CORE_Package (packageID)
go

alter table T_N2L_AccessOverride
   add constraint FK_PersonMaster_From_AccessOverride foreign key (personMasterID)
      references T_PM_PersonMaster (objectID)
go

alter table T_N2L_NonAdminUser
   add constraint FK_PersonMaster_From_NonAdminUser foreign key (personMasterID)
      references T_PM_PersonMaster (objectID)
go

alter table T_PMU_UserAccount
   add constraint FK_PersonMaster_From_UserAccount foreign key (personMasterID)
      references T_PM_PersonMaster (objectID)
go

alter table T_PMU_UserAccount_Preferred
   add constraint FK_PersonMaster_From_UserAccount_Preferred foreign key (personMasterID)
      references T_PM_PersonMaster (objectID)
go

alter table T_PM_CPR
   add constraint FK_Gender_From_CPR foreign key (gender)
      references T_PM_Gender (ID)
go

alter table T_PM_CPR
   add constraint FK_PersonMaster_From_CPR foreign key (personMasterID)
      references T_PM_PersonMaster (objectID)
go

alter table T_PM_PersonMaster
   add constraint FK_ObjectOwner_From_PersonMaster foreign key (objectOwnerID)
      references T_PM_ObjectOwner (objectOwnerID)
go


CREATE FUNCTION fnGK_CORE_DatetimeMaxValue ()
RETURNS datetime
BEGIN
	DECLARE @Result datetime
	SET     @Result = '9999-12-31 23:59:59'
	RETURN  @Result
END
go


CREATE FUNCTION fnGK_CORE_DatetimeMinValue ()
RETURNS datetime
BEGIN
	DECLARE @Result datetime
	SET     @Result = '1753-01-01 00:00:00'
	RETURN  @Result
END
go


CREATE FUNCTION fnGK_CORE_GetConfigValue (@name VARCHAR(60))
RETURNS VARCHAR(1020)
BEGIN
    DECLARE @result_value VARCHAR(1020)
    
    SET @result_value = ''
    
    IF @name IS NULL SET @name = ''
    SET @name = LTRIM( RTRIM( @name ) )
    
    SELECT  @result_value = cfg.value
    FROM    T_CORE_Config cfg
    WHERE   cfg.name = @name
    
    IF @result_value IS NULL SET @result_value = ''
    SET @result_value = LTRIM( RTRIM( @result_value ) )
    
    RETURN @result_value
END
go


CREATE FUNCTION fnGK_CORE_GetStringFromGUID (@arg1 uniqueidentifier)
RETURNS VARCHAR(38)
BEGIN
    IF @arg1 IS NULL RETURN ''
    RETURN CONVERT(VARCHAR(38), @arg1)
END
go


CREATE PROCEDURE spGK_CORE_LogEntry
    @context            VARCHAR(1020),
    @spname             VARCHAR(120),
    @eventID            INTEGER,
    @logText            VARCHAR(2000),
    @logType            INTEGER
AS
BEGIN
    DECLARE @logID      uniqueidentifier,
            @now        DATETIME,
            @package    INTEGER,
            @corrID     VARCHAR(60)
            

    SELECT  @logID      = newid(),
            @now        = GETDATE(),
            @corrID     = ''

    -- ---
    
    -- Prepare parameters
    IF @package IS NULL SET @package = -1   -- Undefined    TODO later ...
    
    IF @context IS NULL SET @context = ''
    IF @spname IS NULL SET @spname = ''
    IF @eventID IS NULL SET @eventID = 0
    IF @logText IS NULL SET @logText = ''
    
    -- ID, logTime, logType, package, context, corrID, spName, eventID, logText
    INSERT INTO T_CORE_Log (logTime, logType, package, context, corrID, spName, eventID, logText)
    --VALUES (@logID, @now, @logType, @package, @context, @corrID, @spname, @eventID, @logText)
    VALUES (@now, @logType, @package, @context, @corrID, @spname, @eventID, @logText)
        
END
go


CREATE PROCEDURE spGK_CORE_LogErrorEntry
    @context            VARCHAR(1020),
    @spname             VARCHAR(120),
    @eventID            INTEGER,
    @logText            VARCHAR(2000)
AS
BEGIN
    EXEC spGK_CORE_LogEntry @context, @spname, @eventID, @logText, 2 -- (2, 'ERROR')
END
go


CREATE PROCEDURE spGK_CORE_ErrorHandler
    @context        VARCHAR(1020)
AS
BEGIN
    DECLARE @errmsg   NVARCHAR(2048),
            @severity TINYINT,
            @state    TINYINT,
            @errno    INTEGER,
            @proc     SYSNAME,
            @lineno   INTEGER,
            @retMsg   NVARCHAR(2048)
           
    SELECT  @errmsg     = error_message(),
            @severity   = error_severity(),
            @state      = error_state(),
            @errno      = error_number(),
            @proc       = error_procedure(),
            @lineno     = error_line()

    IF @errmsg NOT LIKE '***%'
    BEGIN
        SET @retMsg = '***'
        SET @retMsg = @retMsg + ' Error number (' + ltrim(str(@errno)) + ')'
        SET @retMsg = @retMsg + ' thrown in ' + coalesce(quotename(@proc), '<dynamic SQL>')
        SET @retMsg = @retMsg + ' at line (' + ltrim(str(@lineno)) + ').'
        SET @retMsg = @retMsg + ' Error message = [' + @errmsg + '].'
        SET @retMsg = @retMsg + ' Severity/state = (' + ltrim(str(@severity)) + '/' + ltrim(str(@state)) + ').'
        
        SELECT @errmsg = @retMsg
        
        EXEC spGK_CORE_LogErrorEntry @context, @proc, @errno, @errmsg
        
        RAISERROR(@errmsg, @severity, @state)
    END
    ELSE
        RAISERROR(@errmsg, @severity, @state)
END
go


CREATE PROCEDURE spGK_CORE_GetCorrIDFromContext
    @context        VARCHAR(1020)   OUTPUT,
    @corrID         VARCHAR(60)     OUTPUT
AS
BEGIN

    -- If any of the paremeters IS NULL - skip SP silently
    IF @context IS NULL RETURN
    
    -- Otherwise look for the string (CorrID: precisely in context
    
    DECLARE @searchString   VARCHAR(15)
    SELECT  @searchString = '<CorrID>'
    
    DECLARE @ssLen          INTEGER
    SELECT  @ssLen = LEN(@searchString)

    DECLARE @pos1           INTEGER
    
    SELECT @pos1 = CHARINDEX( @searchString, @context )
    
    -- If the corrID key wasn't found, simply skip SP        
    IF @pos1 = 0 RETURN

    -- Otherwise, find the rightmost marker ')'
    
    DECLARE @searchString2   VARCHAR(15)
    SELECT  @searchString2 = '</CorrID>'
    
    DECLARE @pos2           INTEGER
    SELECT @pos2 = CHARINDEX ( @searchString2, @context)

    -- If corrID key closing marker wasn't found, simply skip SP        
    IF @pos2 = 0 RETURN

    -- Otherwise, extract anything between the 2 markers
    SELECT @corrID = SUBSTRING ( @context, @pos1 + @ssLen, @pos2 - (@pos1 + @ssLen) )
    
    SELECT @corrID = LTRIM ( RTRIM ( @corrID ) )

    -- If nothing is in between the markers a GUID corr ID is created and returned in corrID parameter
    IF @corrID = ''
    BEGIN
        DECLARE @corrID2 VARCHAR(38)
        SELECT @corrID2 = dbo.fnGK_CORE_GetStringFromGUID( newid() )

        SELECT @corrID = @corrID2
        
        -- Also in this case ... the @context param must be modified with the newly created GUID type corrID...
        DECLARE @contextTemp VARCHAR(120)
        SELECT @contextTemp = SUBSTRING( @context, 0, @pos1 + @ssLen ) + @corrID + SUBSTRING( @context, @pos2, LEN(@context) )
        SELECT @context = @contextTemp
    END
    
END
go


CREATE PROCEDURE spGK_PMU_PrepareSPInvocation
    @context            VARCHAR(1020) OUTPUT,
    @spname             VARCHAR(120),
    @logText            VARCHAR(2000)
AS
BEGIN
    
    DECLARE @corrID     VARCHAR(60)
    SELECT  @corrID     = ''

    -- ---
    
    EXEC spGK_CORE_GetCorrIDFromContext @context OUTPUT, @corrID OUTPUT
    
    -- EXEC spGK_CORE_LogAuditEntry @context, @spname, 0, @logtext

    -- EXEC spGK_PMU_PrepareSPInvocation @context OUTPUT, spGK_PMU_PrepareSPInvocation, @cprNo

END
go


CREATE PROCEDURE spGK_CORE_GetRuntimeInfo
    @context        VARCHAR(1020),
    @aux            VARCHAR(1020)   OUTPUT
AS
BEGIN TRY
    SET NOCOUNT ON
    SET XACT_ABORT ON
    
    DECLARE @RowCnt                     INTEGER
    DECLARE @ErrNo                      INTEGER
    
    DECLARE @RetVal                     INTEGER
    SELECT  @RetVal         = -1
    
    DECLARE @ErrorSeverity              INTEGER
    SELECT  @ErrorSeverity  = 16
    DECLARE @ErrorState                 INTEGER
    
    -- Declare other parameters
    DECLARE @version                    VARCHAR(250)
    DECLARE @rte                        VARCHAR(250)
    DECLARE @xUser                      VARCHAR(250)
    
    -- ---
    
    EXEC spGK_PMU_PrepareSPInvocation @context OUTPUT, spGK_CORE_GetRuntimeInfo, ''
                
    SELECT @version = '<db-version>' + dbo.fnGK_CORE_GetConfigValue('db-version') + '</db-version>'
    SELECT @rte = '<db-environment>' + dbo.fnGK_CORE_GetConfigValue('db-environment') + '</db-environment>'
    SELECT @xUser = '<db-user>' + CURRENT_USER + '</db-user>'

    SELECT @aux = '<config>' + @version + @rte + @xUser + '</config>'
    
LifeIsGood:
    SELECT  @aux = @aux,
            @RetVal = 0
    
ErrExit:
    IF @RetVal < 0
    BEGIN
        SELECT  @aux = 'There was a problem getting DB runtime info (in SP spGK_CORE_GetRuntimeInfo())!' + @aux,
                @ErrorState = @RetVal * -1
        RAISERROR (@aux, @ErrorSeverity, @ErrorState)
    END

    RETURN @RetVal
END TRY

BEGIN CATCH
    IF @@trancount > 0 ROLLBACK TRANSACTION
    
    EXEC spGK_CORE_ErrorHandler @context
    
    RETURN @RetVal
END CATCH
go


CREATE PROCEDURE spGK_CORE_LogAuditEntry
    @context            VARCHAR(1020),
    @spname             VARCHAR(120),
    @eventID            INTEGER,
    @logText            VARCHAR(2000)
AS
BEGIN
    EXEC spGK_CORE_LogEntry @context, @spname, @eventID, @logText, 1 -- (1, 'AUDIT')
END
go


CREATE PROCEDURE spGK_CORE_LogAuditEntryExt
    @context            VARCHAR(1020),
    @procname           VARCHAR(120),
    @eventID            INTEGER,
    @logText            VARCHAR(2000),
    @aux                VARCHAR(1020) OUTPUT
AS
BEGIN
    SET @aux = ''
    
    EXEC spGK_CORE_LogEntry @context, @procname, @eventID, @logText, 1 -- (1, 'AUDIT')
END
go


CREATE PROCEDURE spGK_CORE_SetConfigValue
    @name       VARCHAR(60),
    @value      VARCHAR(1020)
AS
BEGIN
    IF @name IS NULL SET @name = ''
    SET @name = LTRIM( RTRIM( @name ) )
    
    IF @value IS NULL SET @value = ''
    SET @value = LTRIM( RTRIM( @value ) )
    
    IF @name = '' RETURN
    
    UPDATE  T_CORE_Config
    SET     value = @value
    WHERE   name = @name
    
    IF @@ROWCOUNT = 1 RETURN
    
    INSERT INTO T_CORE_Config VALUES (@name, @value)
    
END
go


CREATE PROCEDURE spGK_CORE_TryCatchSnippet01
    @context    VARCHAR(1020),
    @aux        VARCHAR(1020)   OUTPUT
AS
BEGIN TRY
    SET NOCOUNT ON
    SET XACT_ABORT ON
    
    DECLARE @RowCnt                     INTEGER
    DECLARE @ErrNo                      INTEGER
    
    DECLARE @RetVal                     INTEGER
    SELECT  @RetVal = -1
    
    DECLARE @ErrorSeverity              INTEGER
    SELECT  @ErrorSeverity = 16
    DECLARE @ErrorState                 INTEGER
    
    -- Declare other parameters
    -- ...
    
    -- ---

    -- Prepare parameters
    SET @aux = ''
    
    -- Validate parameters

--    BEGIN TRAN
        --INSERT INTO T_PersonMaster VALUES (@ObjectID, @objectOwnerID, GETDATE())
        --INSERT INTO T_CPR VALUES (EncryptByKey(key_GUID('CprNoEncryptKey'), @cprNo), @birthdate, @gender, @ObjectID, GETDATE())
--    COMMIT TRAN

LifeIsGood:
    SELECT  @aux = '',
            @RetVal = 0
    
ErrExit:
    IF @RetVal < 0
    BEGIN
        SELECT  @aux = 'Error ...! ' + @aux,
                @ErrorState = @RetVal * -1
        RAISERROR (@aux, @ErrorSeverity, @ErrorState)
    END

    --SELECT @RetVal
    RETURN @RetVal
END TRY

BEGIN CATCH
    IF @@trancount > 0 ROLLBACK TRANSACTION
    
    EXEC spGK_CORE_ErrorHandler @context
    
    --SELECT @RetVal
    RETURN @RetVal
END CATCH
go


CREATE PROCEDURE spGK_CORE_ValidateCPR
    @cprNo      VARCHAR(MAX),
    @birthdate  datetime        OUTPUT,
    @gender     INTEGER         OUTPUT,
    @aux        VARCHAR(1020)   OUTPUT
AS
BEGIN TRY
    SET NOCOUNT ON
    SET XACT_ABORT ON
    
    DECLARE @RowCnt                     INTEGER
    DECLARE @ErrNo                      INTEGER
    DECLARE @RetVal                     INTEGER
    SELECT  @RetVal         = -1
    
    DECLARE @ErrorSeverity              INTEGER
    SELECT  @ErrorSeverity  = 16
    DECLARE @ErrorState                 INTEGER    
    
    DECLARE @idx                    INTEGER
    SELECT  @idx = 0
    DECLARE @digit                  VARCHAR(1)
    
    -- ---

    -- Init parameters
    IF @cprNo IS NULL SET @cprNo = ''
    
    SELECT  @cprNo = LTRIM( RTRIM(@cprNo) ),
            @birthdate = GETDATE(),
            @gender = -1,
            @aux = ''
    
    -- Validate CPR number length
    IF LEN(@cprNo) <> 10
    BEGIN
        SET @aux = 'CPR number must be exactly 10 digits in length.'
        GOTO ErrExit
    END

    SET @RetVal = -2
    WHILE @idx < 10
    BEGIN
        SET @digit = SUBSTRING(@cprNo, @idx + 1, 1)
        
        IF @digit < '0' OR @digit > '9' BREAK
        
        SET @idx = @idx + 1
    END
    
    IF @idx < 10
    BEGIN
        SET @aux = 'CPR number must contain digits only.'
        GOTO ErrExit
    END
    
    -- Validate/extract birthdate ...
    SET @RetVal = -3
    DECLARE @day    VARCHAR(2)
    SELECT  @day = SUBSTRING(@cprNo, 1, 2)
    
    DECLARE @month  VARCHAR(2)
    SELECT  @month = SUBSTRING(@cprNo, 3, 2)
    
    DECLARE @year   VARCHAR(2)
    SELECT  @year = SUBSTRING(@cprNo, 5, 2)
    
    DECLARE @date2  VARCHAR(10)
    SELECT  @date2 = @day + '-' + @month + '-' + @year
    
    SET @birthdate = CONVERT(datetime, @date2, 5)
    
    -- ... and gender
    SET @RetVal = -4
    DECLARE @gender2 VARCHAR(1)
    SELECT  @gender2 = SUBSTRING(@cprNo, 10, 1)
    
    DECLARE @gender3 INTEGER
    SELECT  @gender3 = CAST(@gender2 AS INTEGER)

    SET @gender = @gender3 % 2

LifeIsGood:
    SELECT  @aux = '',
            @RetVal = 0
    
ErrExit:
    IF @RetVal < 0
    BEGIN
        SELECT  @aux = 'Invalid CPR number! ' + @aux,
                @ErrorState = @RetVal * -1
        RAISERROR (@aux, @ErrorSeverity, @ErrorState)
    END

    --SELECT @RetVal
    RETURN @RetVal
END TRY

BEGIN CATCH
    IF @@trancount > 0 ROLLBACK TRANSACTION
    
    --EXEC spGK_ErrorHandler
    
    --SELECT @RetVal
    RETURN @RetVal
END CATCH
go


CREATE PROCEDURE spGK_N2L_GetDeploymentPeriodStartStop
    @context        VARCHAR(1020),
    @cprNo          VARCHAR(11),
    @startdate      DATETIME        OUTPUT,
    @stopdate       DATETIME        OUTPUT,
    @aux            VARCHAR(1020)   OUTPUT
AS
BEGIN TRY
    SET NOCOUNT ON
    SET XACT_ABORT ON
    
    DECLARE @RowCnt                     INTEGER
    DECLARE @ErrNo                      INTEGER
    
    DECLARE @RetVal                     INTEGER
    SELECT  @RetVal = -1
    
    DECLARE @ErrorSeverity              INTEGER
    SELECT  @ErrorSeverity = 16
    DECLARE @ErrorState                 INTEGER
    
    DECLARE @birthdate                  DATETIME
    DECLARE @gender                     INTEGER
    
    DECLARE @digit                      VARCHAR(1)
    
    -- Prepare parameters
    SET @aux = ''

    SELECT @startdate = CAST (0 AS DATETIME)
    SELECT @stopdate = CAST (0 AS DATETIME)
    
    -- Validate CPR (and extract birtdate and gender)
    EXEC spGK_CORE_ValidateCPR @cprNo, @birthdate OUTPUT, @gender OUTPUT, @aux OUTPUT

    -- gender param is negative on validation errors
    IF @gender < 0 GOTO ErrExit
    
    -- KMD funny farm rule. If CPR starts with a zero, then zero should be removed.
    -- Under all situations a zero should then be appended as traling digit
    
    SET @digit = LEFT(@cprNo, 1)
    
    IF @digit = '0' SET @cprno = RIGHT(@cprno, 9)
    
    SET @cprno = @cprno + '0'

    SELECT  @RetVal = -2
    
    SELECT @startdate = TILTRÆDELSESDATO, @stopdate = FRATRÆDELSESDATO
    FROM import.LOP_LØNOPLYSNINGER
    WHERE LØNNUMMER = @cprNo
    AND IW_OVERFØRSEL = 
    (
    SELECT MAX(IW_OVERFØRSEL)
    FROM import.LOP_LØNOPLYSNINGER
    WHERE LØNNUMMER = @cprNo    
    AND GETDATE() BETWEEN TILTRÆDELSESDATO AND FRATRÆDELSESDATO
    )
    
    SELECT  @RowCnt = @@ROWCOUNT, @ErrNo = @@ERROR
    IF @ErrNo <> 0 GOTO ErrExit
    
LifeIsGood:
    SELECT  @aux = @aux,
            @RetVal = 0
    
ErrExit:
    IF @RetVal < 0
    BEGIN
        SELECT  @aux = 'Procedure spGK_N2L_GetDeploymentPeriodStartStop() failed unexpectedly! ' + @aux,
                @ErrorState = @RetVal * -1
        RAISERROR (@aux, @ErrorSeverity, @ErrorState)
    END

    RETURN @RetVal
END TRY

BEGIN CATCH
    IF @@trancount > 0 ROLLBACK TRANSACTION
    
    EXEC spGK_CORE_ErrorHandler @context
    
    RETURN @RetVal
END CATCH
go


CREATE PROCEDURE spGK_PM_PrepareSPInvocation
    @context            VARCHAR(1020) OUTPUT,
    @spname             VARCHAR(120),
    @logText            VARCHAR(2000)
AS
BEGIN
    
    DECLARE @corrID     VARCHAR(60)
    SELECT  @corrID     = ''

    -- ---
    
    EXEC spGK_CORE_GetCorrIDFromContext @context OUTPUT, @corrID OUTPUT
    -- EXEC spGK_CORE_LogAuditEntry @context, @spname, 0, @logtext

END
go


CREATE PROCEDURE spGK_PM_GetOwnerIDFromNamespace
    @context                    VARCHAR(1020),
    @objectOwnerID              uniqueidentifier    OUTPUT,
    @objectOwnerNamespace       VARCHAR(510),
    @aux                        VARCHAR(1020)       OUTPUT
AS
BEGIN TRY
    SET NOCOUNT ON
    SET XACT_ABORT ON
    
    DECLARE @RowCnt                     INTEGER
    DECLARE @ErrNo                      INTEGER
    
    DECLARE @RetVal                     INTEGER
    SELECT  @RetVal = -1
    
    DECLARE @ErrorSeverity              INTEGER
    SELECT  @ErrorSeverity = 16
    DECLARE @ErrorState                 INTEGER    

    DECLARE @create_on_non_existence    INTEGER
    SELECT  @create_on_non_existence = 0
    
    DECLARE @objectOwnerIDTry       uniqueidentifier
    
    -- ---
    
    EXEC spGK_PM_PrepareSPInvocation @context OUTPUT, spGK_PM_GetOwnerIDFromNamespace, @objectOwnerNamespace
    
    -- Prepare parameters
    -- SET @objectOwnerID = NULL
    
    IF @objectOwnerNamespace IS NULL SET @objectOwnerNamespace = ''
    SET @objectOwnerNamespace = LTRIM( RTRIM( @objectOwnerNamespace ) )
    
    SET @aux = UPPER ( LTRIM ( RTRIM (@aux) ) )
    IF CHARINDEX('CREATE-ON-NON-EXISTENCE', @aux) > 0 SET @create_on_non_existence = 1
    
    SET @aux = ''
    
    
    -- Validate input params (ie namespace is supplied)
    IF @objectOwnerNamespace = ''
    BEGIN
        SET @aux = 'A non-empty namespace parameter (@objectOwnerNamespace) MUST be specified.'
        GOTO ErrExit
    END
    

    -- If nanmespace already exists, get objectOwnerID ...
    SET @RetVal = -2
    SELECT  @objectOwnerIDTry = objowner.objectOwnerID
    FROM    T_PM_ObjectOwner objowner
    WHERE   objowner.ownerNamespace = @objectOwnerNamespace
    SELECT  @RowCnt = @@ROWCOUNT, @ErrNo = @@ERROR
    
    IF @RowCnt = 1
    BEGIN
        -- ... and validate that objectOwnerID input param is either set to NULL
        -- or equals the value of @objectOwnerIDTry returned above.
        IF NOT (@objectOwnerID IS NULL OR @objectOwnerID = @objectOwnerIDTry)
        BEGIN
            SET @aux = ''
            SET @aux = @aux + 'The specified parameter for object owner ID (@objectOwnerID=[' + dbo.fnGK_GetStringFromGUID(@objectOwnerID)
            SET @aux = @aux + ']) is different from the object owner ID (@objectOwnerIDTry=[' + dbo.fnGK_GetStringFromGUID(@objectOwnerIDTry)
            SET @aux = @aux + ']) already stored in the database (namespace=['+ @objectOwnerNamespace +']).'
            GOTO ErrExit
        END
        
        -- Life is good
        SELECT @objectOwnerID = @objectOwnerIDTry, @aux = ''
        GOTO LifeIsGood
    END

    SET @RetVal = -3
    
    IF @create_on_non_existence = 1
    BEGIN
        -- If no @objectOwnerID is specified in call, generate it here
        IF @objectOwnerID IS NULL SET @objectOwnerID = newid()
    
        -- No object owner record was found, so create one...
        SET @RetVal = -4
        INSERT  INTO T_PM_ObjectOwner VALUES (@objectOwnerID, @objectOwnerNamespace, GETDATE())
        
        SET @aux = 'CREATED'
    END    
    ELSE
    BEGIN
        SET @aux = ''
        SET @aux = @aux + 'The specified namespace (namespace=['+ @objectOwnerNamespace +']) was NOT found.'
        
        -- Life sucks - namespace not found
        GOTO ErrExit
    END
    
LifeIsGood:
    SELECT  @aux = '',
            @RetVal = 0
        
ErrExit:
    IF @RetVal < 0
    BEGIN
        SELECT  @aux = 'Retrieval of object owner ID failed! ' + @aux,
                @ErrorState = @RetVal * -1
        RAISERROR (@aux, @ErrorSeverity, @ErrorState)
    END

    RETURN @RetVal
END TRY

BEGIN CATCH
    IF @@trancount > 0 ROLLBACK TRANSACTION
    
    EXEC spGK_CORE_ErrorHandler @context
    
    RETURN @RetVal
END CATCH
go


CREATE PROCEDURE spGK_PM_GetObjectIDFromCPR
    @context            VARCHAR(1020),
    @cprNo              VARCHAR(10),
    @objectOwnerID      uniqueidentifier,
    @objectID           uniqueidentifier    OUTPUT,
    @aux                VARCHAR(1020)       OUTPUT
AS
BEGIN TRY
    SET NOCOUNT ON
    SET XACT_ABORT ON
    
    DECLARE @RowCnt                     INTEGER
    DECLARE @ErrNo                      INTEGER
    
    DECLARE @RetVal                     INTEGER
    SELECT  @RetVal         = -1
    
    DECLARE @ErrorSeverity              INTEGER
    SELECT  @ErrorSeverity  = 16
    DECLARE @ErrorState                 INTEGER    

    DECLARE @birthdate                  DATETIME
    DECLARE @gender                     INTEGER
    
    DECLARE @objectOwnerNamespace       VARCHAR(510)
    
    DECLARE @encryptedCprNo             VARBINARY(90)
    
    -- ---
    
    EXEC spGK_PM_PrepareSPInvocation @context OUTPUT, spGK_PM_GetObjectIDFromCPR, @cprNo
                    
    -- Prepare parameters
    SET @ObjectID = NULL
    SET @aux = ''
    
    -- Validate CPR (and extract birtdate and gender)
    EXEC spGK_CORE_ValidateCPR @cprNo, @birthdate OUTPUT, @gender OUTPUT, @aux OUTPUT

    -- gender param is negative on validation errors
    IF @gender < 0 GOTO ErrExit
    
    -- Open key to be used for encrypting CPR
    SET @RetVal = -2
    OPEN SYMMETRIC KEY CprNoEncryptKey DECRYPTION BY CERTIFICATE CertForEncryptOfCprNoKey;

    -- Get the object ID that correspond to the specified CPR (iff any). @ObjectID is NULL if no match is found.
    SET @RetVal = -3
    SELECT  @ObjectID = pm.ObjectID
    FROM    T_PM_CPR cpr, T_PM_PersonMaster pm
    WHERE   cpr.cprNo = @cprNo
    AND     cpr.personMasterID = pm.objectID
    
    IF @ObjectID IS NULL
    BEGIN
        -- No CPR entry was found
        
        SET @RetVal = -4
        
        -- If object owner ID is NULL (unspecified) - get the owner ID for the self namespace configured for this installation
        IF @objectOwnerID IS NULL
        BEGIN
            SET @RetVal = -5
            SET @aux = 'ObjectOwner ID was not specified in call, and NO default namespace (namespace-self) was defined in the config table (T_CORE_Config). Verify that DB has been initalized (spGK_InitDB).'
            SET @objectOwnerNamespace = dbo.fnGK_CORE_GetConfigValue('namespace-self')
            
            IF LEN(@objectOwnerNamespace) = 0 GOTO ErrExit
            
            SET @RetVal = -6
            EXEC spGK_PM_GetOwnerIDFromNamespace @context, @objectOwnerID OUTPUT, @objectOwnerNamespace, @aux OUTPUT
        END
        
        -- Create new entry in PersonMaster table
        SET @ObjectID = newid()
        
        SET @RetVal = -7
        
        BEGIN TRAN
            INSERT INTO T_PM_PersonMaster VALUES (@ObjectID, @objectOwnerID, GETDATE())
            INSERT INTO T_PM_CPR VALUES (EncryptByKey(key_GUID('CprNoEncryptKey'), @cprNo), @birthdate, @gender, @ObjectID, GETDATE(), @cprNo)
        COMMIT TRAN
    END
    
LifeIsGood:
    SELECT  @aux = '',
            @RetVal = 0
        
ErrExit:
    IF @RetVal < 0
    BEGIN
        SELECT  @aux = 'Retrieval of object ID FROM CPR failed! ' + @aux,
                @ErrorState = @RetVal * -1
        RAISERROR (@aux, @ErrorSeverity, @ErrorState)
    END

    RETURN @RetVal
END TRY

BEGIN CATCH
    IF @@trancount > 0 ROLLBACK TRANSACTION
    
    EXEC spGK_CORE_ErrorHandler @context
    
    RETURN @RetVal
END CATCH

go

CREATE PROCEDURE [dbo].[spGK_PM_GetObjectIDsFromCPRArray]
    @context            VARCHAR(1020),
    @cprNoArray         VARCHAR(MAX),
    @objectOwnerID      uniqueidentifier,
	@aux                VARCHAR(1020)       OUTPUT	
    
AS
BEGIN TRY
    SET NOCOUNT ON
    SET XACT_ABORT ON
    
    DECLARE @RowCnt                     INTEGER
    DECLARE @ErrNo                      INTEGER
    
    DECLARE @RetVal                     INTEGER
    SELECT  @RetVal         = -1
    
    DECLARE @ErrorSeverity              INTEGER
    SELECT  @ErrorSeverity  = 16
    DECLARE @ErrorState                 INTEGER    

    DECLARE @birthdate                  DATETIME
    DECLARE @gender                     INTEGER
    
    DECLARE @objectOwnerNamespace       VARCHAR(510)
    
	DECLARE @cprNo                      VARCHAR(MAX)
	DECLARE @index			            INT
	DECLARE @ReturnTable                TABLE  (ID INT IDENTITY, CprNo VARCHAR(MAX), Birthdate DATETIME, Gender INT, ObjectID UNIQUEIDENTIFIER, Existing BIT DEFAULT 0, Aux VARCHAR(1020))
        
	-- ---
    
    EXEC spGK_PM_PrepareSPInvocation @context OUTPUT, spGK_PM_GetObjectIDsFromCPRArray, @cprNoArray
                    
    -- Prepare parameters
    DECLARE @objectID           uniqueidentifier
	
    -- Open key to be used for encrypting CPR
	SET @RetVal = -2
	OPEN SYMMETRIC KEY CprNoEncryptKey DECRYPTION BY CERTIFICATE CertForEncryptOfCprNoKey;
	
	-- If object owner ID is NULL (unspecified) - get the owner ID for the self namespace configured for this installation
	IF @objectOwnerID IS NULL
	BEGIN
		SET @RetVal = -3
		SET @aux = 'ObjectOwner ID was not specified in call, and NO default namespace (namespace-self) was defined in the config table (T_CORE_Config). Verify that DB has been initalized (spGK_InitDB).'
		SET @objectOwnerNamespace = dbo.fnGK_CORE_GetConfigValue('namespace-self')
    
		IF LEN(@objectOwnerNamespace) = 0 GOTO ErrExit
    
		SET @RetVal = -4
		EXEC spGK_PM_GetOwnerIDFromNamespace @context, @objectOwnerID OUTPUT, @objectOwnerNamespace, @aux OUTPUT
	END
		
	SET @RetVal = -5
    -- Split the CPR numbers array and validate elements
	-- Invalid Cpr numbers will have Gender < 0
    WHILE LEN(@cprNoArray) > 0
	BEGIN
		SET @RetVal = -6
		SET @ObjectID = NULL
		SET @aux = ''
		SET @index = CHARINDEX (',' , @cprNoArray)
		IF @INDEX > 0
			BEGIN
				SET @cprNo = SUBSTRING(@cprNoArray , 1 , @index - 1)
				SET @cprNoArray = SUBSTRING(@cprNoArray , @index + 1 , LEN(@cprNoArray) - @index)
			END
		ELSE
			BEGIN
				SET @CprNo = @cprNoArray
				SET @cprNoArray = ''
			END
		
		SET @RetVal = -7
		-- Validate CPR (and extract birtdate and gender)
		BEGIN TRY
			EXEC spGK_CORE_ValidateCPR @cprNo, @birthdate OUTPUT, @gender OUTPUT, @aux OUTPUT
		END TRY
		BEGIN CATCH
			-- gender param is negative on validation errors. Do nothing
		END CATCH
		
		SET @RetVal = -8
		INSERT INTO @ReturnTable (CprNo, Birthdate, Gender, Aux) VALUES (@cprNo, @birthdate, @gender, @aux)
	END
	
	SET @RetVal = -9
	-- Existing ObjectIDs
	UPDATE RET	
	SET ObjectID = pm.ObjectID, Existing = 1
	FROM @ReturnTable RET, T_PM_CPR cpr, T_PM_PersonMaster pm
	WHERE 
			RET.Gender >= 0
			AND cpr.cprNo = RET.CprNo
			AND     cpr.personMasterID = pm.objectID
	
	SET @RetVal = -10
	-- New ObjctIDs
	UPDATE @ReturnTable
	SET ObjectID = NEWID()
	WHERE Gender >= 0
		AND Existing = 0
	
	BEGIN TRAN
		SET @RetVal = -11
		
		INSERT INTO T_PM_PersonMaster 
		SELECT ObjectID, @objectOwnerID, GETDATE() 
		FROM @ReturnTable 
		WHERE Gender >= 0
			AND Existing = 0
		
		SET @RetVal = -12
		INSERT INTO T_PM_CPR 
		SELECT EncryptByKey(key_GUID('CprNoEncryptKey'), CprNo), Birthdate, Gender, ObjectID, GETDATE(), CprNo
		FROM @ReturnTable 
		WHERE Gender >= 0
			AND Existing = 0
		
		SET @RetVal = -13
	COMMIT TRAN
	
	LifeIsGood:
		SELECT  @aux = '',
				@RetVal = 0
        
	ErrExit:
		IF @RetVal < 0
		BEGIN
			SELECT  @aux = 'Retrieval of object ID FROM CPR failed! ' + @aux,
					@ErrorState = @RetVal * -1
			RAISERROR (@aux, @ErrorSeverity, @ErrorState)
		END
	
	SELECT * FROM @ReturnTable
	RETURN @RetVal
END TRY

BEGIN CATCH
    IF @@trancount > 0 ROLLBACK TRANSACTION
    
    EXEC spGK_CORE_ErrorHandler @context
    
    RETURN @RetVal
END CATCH

go


CREATE PROCEDURE spGK_N2L_IsRegisteredExplicitAccess
    @context            VARCHAR(1020),
    @cprNo              VARCHAR(10),
    @registered         INTEGER         OUTPUT,
    @accessAllowed      INTEGER         OUTPUT,
    @aux                VARCHAR(1020)   OUTPUT
AS
BEGIN TRY
    SET NOCOUNT ON
    SET XACT_ABORT ON
    
    DECLARE @RowCnt                     INTEGER
    DECLARE @ErrNo                      INTEGER
    
    DECLARE @RetVal                     INTEGER
    SELECT  @RetVal         = -1
    
    DECLARE @ErrorSeverity              INTEGER
    SELECT  @ErrorSeverity  = 16
    DECLARE @ErrorState                 INTEGER
    
    -- Declare other parameters
    DECLARE @objectOwnerID              uniqueidentifier
    SELECT  @objectOwnerID  = NULL
    DECLARE @objectID                   uniqueidentifier

    -- Init parameters
    SELECT  @aux = '',
            @registered = 0,
            @accessAllowed = 0
    
    -- Validate parameters
    SET @RetVal = -2

    EXEC spGK_PM_GetObjectIDFromCPR @context, @cprNo, @objectOwnerID, @objectID OUTPUT, @aux OUTPUT
    
    IF @objectID IS NULL
    BEGIN
        SET @aux = 'CPR (' + @cprNo + ') was not found/could not be created. aux=[' + @aux + ']'
        GOTO ErrExit
    END
    
    SET @RetVal = -3
    
    SELECT  @accessAllowed = ao.accessAllowed
    FROM    T_N2L_AccessOverride ao
    WHERE   ao.personMasterID = @objectID
    
    SELECT  @RowCnt = @@ROWCOUNT, @ErrNo = @@ERROR
    
    IF @accessAllowed IS NULL SET @accessAllowed = 0
    
LifeIsGood:
    SELECT  @aux = '',
            @RetVal = @RowCnt,
            @registered = @RowCnt
            
ErrExit:
    IF @RetVal < 0
    BEGIN
        SELECT  @aux = 'Query on explicit user access registration failed (in spGK_N2L_IsRegisteredExplicitAccess())! ' + @aux,
                @ErrorState = @RetVal * -1
        RAISERROR (@aux, @ErrorSeverity, @ErrorState)
    END

    RETURN @RetVal
    
END TRY

BEGIN CATCH
    IF @@trancount > 0 ROLLBACK TRANSACTION
    
    EXEC spGK_CORE_ErrorHandler @context
    
    RETURN @RetVal
END CATCH
go


CREATE PROCEDURE spGK_N2L_IsRegisteredNonAdminUser
    @context            VARCHAR(1020),
    @cprNo              VARCHAR(10),
    @registered         INTEGER         OUTPUT,
    @aux                VARCHAR(1020)   OUTPUT
AS
BEGIN TRY
    SET NOCOUNT ON
    SET XACT_ABORT ON
    
    DECLARE @RowCnt                     INTEGER
    DECLARE @ErrNo                      INTEGER
    
    DECLARE @RetVal                     INTEGER
    SELECT  @RetVal =       -1
    
    DECLARE @ErrorSeverity              INTEGER
    SELECT  @ErrorSeverity  = 16
    DECLARE @ErrorState                 INTEGER
    
    -- Declare other parameters
    DECLARE @objectOwnerID              uniqueidentifier
    SELECT  @objectOwnerID  = NULL
    DECLARE @objectID                   uniqueidentifier

    -- Init parameters
    SELECT  @aux = '',
            @registered = 0
    
    -- Validate parameters
    SET @RetVal = -2

    EXEC spGK_PM_GetObjectIDFromCPR @context, @cprNo, @objectOwnerID, @objectID OUTPUT, @aux OUTPUT
    
    IF @objectID IS NULL
    BEGIN
        SET @aux = 'CPR (' + @cprNo + ') was not found/could not be created. aux=[' + @aux + ']'
        GOTO ErrExit
    END
    
    SET @RetVal = -3
    
    SELECT  @RowCnt = COUNT(nau.personMasterID)
    FROM    T_N2L_NonAdminUser nau
    WHERE   nau.personMasterID = @objectID
    
LifeIsGood:
    SELECT  @aux = '',
            @RetVal = @RowCnt,
            @registered = @RowCnt
            
ErrExit:
    IF @RetVal < 0
    BEGIN
        SELECT  @aux = 'Procedure spGK_N2L_IsRegisteredNonAdminUser() failed unexpectedly! ' + @aux,
                @ErrorState = @RetVal * -1
        RAISERROR (@aux, @ErrorSeverity, @ErrorState)
    END

    RETURN @RetVal
    
END TRY

BEGIN CATCH
    IF @@trancount > 0 ROLLBACK TRANSACTION
    
    EXEC spGK_CORE_ErrorHandler @context
    
    RETURN @RetVal
END CATCH
go


CREATE PROCEDURE spGK_N2L_RegisterExplicitAccess
    @context            VARCHAR(1020),
    @cprNo              VARCHAR(10),
    @register           INTEGER,
    @accessAllowed      INTEGER,
    @aux                VARCHAR(1020)   OUTPUT
AS
BEGIN TRY
    SET NOCOUNT ON
    SET XACT_ABORT ON
    
    DECLARE @RowCnt                     INTEGER
    DECLARE @ErrNo                      INTEGER
    
    DECLARE @RetVal                     INTEGER
    SELECT  @RetVal         = -1
    
    DECLARE @ErrorSeverity              INTEGER
    SELECT  @ErrorSeverity  = 16
    DECLARE @ErrorState                 INTEGER
    
    DECLARE @objectOwnerID              uniqueidentifier
    SELECT  @objectOwnerID  = NULL
    DECLARE @objectID                   uniqueidentifier
    
    -- Init NULL parameters to false for NO access
    IF @register IS NULL SET @register = 1
    IF @accessAllowed IS NULL SET @accessAllowed = 0
    
    -- Make sure that both parameters is etiher set to zero or one.
    IF @register <> 0 SET @register = 1
    IF @accessAllowed <> 0 SET @accessAllowed = 1
    
    SET @aux = ''

    SET @RetVal = -3
    
    EXEC spGK_PM_GetObjectIDFromCPR @context, @cprNo, @objectOwnerID, @objectID OUTPUT, @aux OUTPUT

    IF @objectID IS NULL
    BEGIN
        SET @aux = 'CPR (' + @cprNo + ') was not found/could not be created. aux=[' + @aux + ']'
        GOTO ErrExit
    END

    SET @RetVal = -4
        
    -- Now, delete any existing record
    DELETE
    FROM T_N2L_AccessOverride
    WHERE personMasterId = @objectID

    SET @RetVal = -5
        
    -- If the request was to register the person as with a black- or white lister marker, insert a new record
    IF @register = 1
    BEGIN
        INSERT INTO T_N2L_AccessOverride VALUES ( @objectID, GETDATE(), @accessAllowed )
    END
        
LifeIsGood:
    SELECT  @aux = '',
            @RetVal = 0
    
ErrExit:
    IF @RetVal < 0
    BEGIN
        SELECT  @aux = 'It was not possible to register/deregister person on black/whitelist! ' + @aux,
                @ErrorState = @RetVal * -1
        RAISERROR (@aux, @ErrorSeverity, @ErrorState)
    END

    RETURN @RetVal
END TRY

BEGIN CATCH
    IF @@trancount > 0 ROLLBACK TRANSACTION
    
    EXEC spGK_CORE_ErrorHandler @context
    
    RETURN @RetVal
END CATCH
go


CREATE PROCEDURE spGK_N2L_RegisterNonAdminUser
    @context            VARCHAR(1020),
    @cprNo              VARCHAR(10),
    @register           INTEGER,
    @aux                VARCHAR(1020)   OUTPUT
AS
BEGIN TRY
    SET NOCOUNT ON
    SET XACT_ABORT ON
    
    DECLARE @RowCnt                     INTEGER
    DECLARE @ErrNo                      INTEGER
    
    DECLARE @RetVal                     INTEGER
    SELECT  @RetVal         = -1
    
    DECLARE @ErrorSeverity              INTEGER
    SELECT  @ErrorSeverity  = 16
    DECLARE @ErrorState                 INTEGER
    
    DECLARE @objectOwnerID              uniqueidentifier
    SELECT  @objectOwnerID  = NULL
    DECLARE @objectID                   uniqueidentifier
    
    -- Init register parameters to false
    IF @register IS NULL SET @register = 0
    
    SET @aux = ''
    
    SET @RetVal = -3
    EXEC spGK_PM_GetObjectIDFromCPR @context, @cprNo, @objectOwnerID, @objectID OUTPUT, @aux OUTPUT

    IF @objectID IS NULL
    BEGIN
        SET @aux = 'CPR (' + @cprNo + ') was not found/could not be created. aux=[' + @aux + ']'
        GOTO ErrExit
    END
    
    SET @RetVal = -4
        
    -- Now, delete any existing record
    DELETE
    FROM T_N2L_NonAdminUser
    WHERE personMasterId = @objectID

    SET @RetVal = -5
        
    -- If the request was to register the person as non admin user, create a new record
    IF @register = 1
    BEGIN
        INSERT INTO T_N2L_NonAdminUser VALUES ( @objectID, GETDATE() )
    END
    
LifeIsGood:
    SELECT  @aux = '',
            @RetVal = 0
    
ErrExit:
    IF @RetVal < 0
    BEGIN
        SELECT  @aux = 'It was not possible to register/deregister non administrative user by CPR number! ' + @aux,
                @ErrorState = @RetVal * -1
        RAISERROR (@aux, @ErrorSeverity, @ErrorState)
    END

    RETURN @RetVal
END TRY

BEGIN CATCH
    IF @@trancount > 0 ROLLBACK TRANSACTION
    
    EXEC spGK_CORE_ErrorHandler @context
    
    RETURN @RetVal
END CATCH
go


CREATE PROCEDURE spGK_PMU_GetCurrentLoginRecord
    @context            VARCHAR(1020),
    @loginName          VARCHAR(30),
    @objectID           uniqueidentifier    OUTPUT,
    @personMasterID     uniqueidentifier    OUTPUT,
    @transactFrom       DATETIME            OUTPUT,
    @aux                VARCHAR(1020)       OUTPUT
AS
BEGIN TRY
    SET NOCOUNT ON
    SET XACT_ABORT ON
    
    DECLARE @RowCnt                     INTEGER
    DECLARE @ErrNo                      INTEGER
    
    DECLARE @RetVal                     INTEGER
    SELECT  @RetVal         = -1
    
    DECLARE @ErrorSeverity              INTEGER
    SELECT  @ErrorSeverity  = 16
    DECLARE @ErrorState                 INTEGER
    
    DECLARE @transactToMax              DATETIME
    SELECT  @transactToMax  = dbo.fnGK_core_DatetimeMaxValue()
    
    -- ---
    
    EXEC spGK_PMU_PrepareSPInvocation @context OUTPUT, spGK_PMU_GetCurrentLoginRecord, @loginName
                
    -- Init parameters
    SELECT  @aux = ''

    -- Get current record defined as the record with transactTo set to infinity
    SELECT  @objectID = ua.objectID, @personMasterID = ua.personMasterID, @transactFrom = ua.transactFrom
    FROM    T_PMU_UserAccount ua
    WHERE   ua.loginName = @loginName
    AND     ua.transactTo = @transactToMax
    
    SELECT  @RowCnt = @@ROWCOUNT, @ErrNo = @@ERROR
    
LifeIsGood:
    SELECT  @aux = '',
            @RetVal = @RowCnt
        
ErrExit:
    IF @RetVal < 0
    BEGIN
        SELECT  @aux = 'Current user account record could NOT be retrived from loginname=(' + @loginName + ') (in SP spGK_PMU_GetCurrentLoginRecord())! ' + @aux,
                @ErrorState = @RetVal * -1
        RAISERROR (@aux, @ErrorSeverity, @ErrorState)
    END

    RETURN @RetVal
    
END TRY

BEGIN CATCH
    IF @@trancount > 0 ROLLBACK TRANSACTION
    
    EXEC spGK_CORE_ErrorHandler @context
    
    RETURN @RetVal
END CATCH
go


CREATE PROCEDURE spGK_PMU_DeleteLoginName
    @context            VARCHAR(1020),
    @loginName          VARCHAR(30),
    @aux                VARCHAR(1020)   OUTPUT
AS
BEGIN TRY
    SET NOCOUNT ON
    SET XACT_ABORT ON
    
    DECLARE @RowCnt                     INTEGER
    DECLARE @ErrNo                      INTEGER
    
    DECLARE @RetVal                     INTEGER
    SELECT  @RetVal         = -1
    
    DECLARE @ErrorSeverity              INTEGER
    SELECT  @ErrorSeverity  = 16
    DECLARE @ErrorState                 INTEGER

    DECLARE @uaObjectID                 uniqueidentifier
    DECLARE @personMasterIDRef          uniqueidentifier

    DECLARE @transactFrom               DATETIME
    DECLARE @transactNow                DATETIME
    DECLARE @transactToMax              DATETIME
    
    SELECT  @transactFrom   = GETDATE(),
            @transactNow    = GETDATE(),
            @transactToMax  = dbo.fnGK_core_DatetimeMaxValue()
    
    -- ---
    
    EXEC spGK_PMU_PrepareSPInvocation @context OUTPUT, spGK_PMU_DeleteLoginName, @loginName
    
    -- Init parameters
    SELECT  @aux = ''
    
    -- Validate that loginname is not empty OR NULL
    IF @loginName IS NULL SET @loginName = ''
    SET @loginName = LTRIM( RTRIM(@loginName) )
        
    IF @loginName = ''
    BEGIN
        SET @aux = 'loginName parameter is unspecified/empty! '
        GOTO ErrExit
    END
    
    SELECT  @RetVal = -2
    
    -- Validate that old login, actually exists
    EXEC @RowCnt = spGK_PMU_GetCurrentLoginRecord @context, @loginName, @uaObjectID OUTPUT, @personMasterIDRef OUTPUT, @transactFrom OUTPUT, @aux OUTPUT

    IF @RowCnt <> 1
    BEGIN
        SET @aux = 'Loginname (' + @loginName + ') was not found in the database! '
        GOTO ErrExit
    END
    
    SELECT  @RetVal = -3
    
    -- Update (retire) current user account record
    UPDATE  T_PMU_UserAccount
    SET     transactTo = @transactNow
    WHERE   objectId = @uaObjectID
    AND     transactTo = @transactToMax
    
    SELECT  @RowCnt = @@ROWCOUNT, @ErrNo = @@ERROR
    IF @RowCnt <> 1
    BEGIN
        SET @aux = 'UNKNOWN STATE: User account record could NOT be retired/updated! '
        GOTO ErrExit
    END
    
LifeIsGood:
    SELECT  @aux = '',
            @RetVal = @RowCnt
        
ErrExit:
    IF @RetVal < 0
    BEGIN
        SELECT  @aux = 'Loginname deletion failed (in SP spGK_PMU_DeleteLoginName()). User account objectID=(' + dbo.fnGK_GetStringFromGUID(@uaObjectID) + '), loginname=(' + @loginName + ')! ' + @aux,
                @ErrorState = @RetVal * -1
        RAISERROR (@aux, @ErrorSeverity, @ErrorState)
    END

    RETURN @RetVal
END TRY

BEGIN CATCH
    IF @@trancount > 0 ROLLBACK TRANSACTION
    
    EXEC spGK_CORE_ErrorHandler @context
    
    RETURN @RetVal
END CATCH
go


CREATE PROCEDURE spGK_PMU_GetCPRFromLoginName
    @context            VARCHAR(1020),
    @loginName          VARCHAR(30),
    @cprNo              VARCHAR(10)     OUTPUT,
    @aux                VARCHAR(1020)   OUTPUT
AS
BEGIN TRY
    SET NOCOUNT ON
    SET XACT_ABORT ON
    
    DECLARE @RowCnt                     INTEGER
    DECLARE @ErrNo                      INTEGER
    
    DECLARE @RetVal                     INTEGER
    SELECT  @RetVal         = -1
    
    DECLARE @ErrorSeverity              INTEGER
    SELECT  @ErrorSeverity  = 16
    DECLARE @ErrorState                 INTEGER
    
    DECLARE @uaObjectID                 uniqueidentifier
    DECLARE @personMasterID             uniqueidentifier
    
    DECLARE @transactFrom               DATETIME
    DECLARE @transactToMax              DATETIME
    
    SELECT  @transactFrom   = GETDATE(),
            @transactToMax  = dbo.fnGK_core_DatetimeMaxValue()
            
    DECLARE @cprNoEncrypted             VARBINARY(90)
    
    -- ---
    
    EXEC spGK_PMU_PrepareSPInvocation @context OUTPUT, spGK_PMU_GetCPRFromLoginName, @loginName
    
    -- Prepare parameters
    SELECT  @cprNo = '',
            @aux = ''
    
    -- Validate parameters
    IF @loginName = ''
    BEGIN
        SET @aux = 'loginName parameter is unspecified/empty.'
        GOTO ErrExit
    END
    
    -- Login names are stord in lowercase
    SET @loginName = LOWER(@loginName)
    
    EXEC @RowCnt = spGK_PMU_GetCurrentLoginRecord @context, @loginName, @uaObjectID OUTPUT, @personMasterID OUTPUT, @transactFrom OUTPUT, @aux OUTPUT
    IF @RowCnt = 0 GOTO LifeIsGood -- but no loginname was found!!
    
    -- Open key to be used for encrypting CPR
    OPEN SYMMETRIC KEY CprNoEncryptKey DECRYPTION BY CERTIFICATE CertForEncryptOfCprNoKey;
     
    -- Now retrieve CPR
    SELECT  @cprNoEncrypted = cpr.encryptedCprNo
    FROM    T_PMU_UserAccount ua, T_PM_CPR cpr
    WHERE   ua.objectID = @uaObjectID
    AND     ua.transactTo = @transactToMax
    AND     ua.PersonMasterID = cpr.PersonMasterID
    
    SET @RowCnt = @@ROWCOUNT
    
    IF @cprNoEncrypted IS NOT NULL
    BEGIN
        SET @cprNo = DecryptByKey(@cprNoEncrypted)
    END

LifeIsGood:
    SELECT  @aux = '',
            @RetVal = @RowCnt
    
ErrExit:
    IF @RetVal < 0
    BEGIN
        SELECT  @aux = 'CPR number could not be retrived from login name! ' + @aux,
                @ErrorState = @RetVal * -1
        RAISERROR (@aux, @ErrorSeverity, @ErrorState)
    END

    SELECT  cpr.cprNo, cpr.encryptedCprNo, cpr.birthDate, cpr.gender, cpr.personMasterID, cpr.createTS
    FROM    T_PMU_UserAccount ua, T_PM_CPR cpr
    WHERE   ua.objectID = @uaObjectID
    AND     ua.transactTo = @transactToMax
    AND     ua.PersonMasterID = cpr.PersonMasterID
    
    RETURN @RetVal
END TRY

BEGIN CATCH
    IF @@trancount > 0 ROLLBACK TRANSACTION
    
    EXEC spGK_CORE_ErrorHandler @context
    
    SELECT @RetVal
    RETURN @RetVal
END CATCH
go


CREATE PROCEDURE spGK_PMU_InsertDefaultPreferred
    @personMasterID     uniqueidentifier
AS
BEGIN
    DECLARE @transactToMax              DATETIME
    SELECT  @transactToMax  = dbo.fnGK_core_DatetimeMaxValue()
            
    -- ---
    
    INSERT INTO T_PMU_UserAccount_Preferred (personMasterID, uaObjectID)
    
    SELECT TOP 1 ua.personMasterID, ua.objectID
    FROM   T_PMU_UserAccount ua
    WHERE  ua.personmasterID = @personMasterID
    AND    ua.transactTo = @transactToMax
    
    ORDER BY ua.transactFrom ASC

END
go


CREATE PROCEDURE spGK_PMU_VerifyPreferredUA
    @context            VARCHAR(1020),
    @personMasterID     uniqueidentifier,
    @aux                VARCHAR(1020)   OUTPUT
AS
BEGIN TRY
    SET NOCOUNT ON
    SET XACT_ABORT ON
    
    DECLARE @RowCnt                     INTEGER
    DECLARE @ErrNo                      INTEGER
    
    DECLARE @RetVal                     INTEGER
    SELECT  @RetVal = -1
    
    DECLARE @ErrorSeverity              INTEGER
    SELECT  @ErrorSeverity  = 16
    DECLARE @ErrorState                 INTEGER
    
    DECLARE @ObjectIDTry                uniqueidentifier
    
    DECLARE @transactFrom               DATETIME
    DECLARE @transactNow                DATETIME
    DECLARE @transactToMax              DATETIME
    
    SELECT  @transactFrom   = GETDATE(),
            @transactNow    = GETDATE(),
            @transactToMax  = dbo.fnGK_core_DatetimeMaxValue()
            
    DECLARE @cprNoTry       VARCHAR(10)
    
    -- ---
    
    -- Init relevant parameters
    SET @aux = ''
    
    -- Verify that at least ONE current UA record actually exists for this person
    SELECT  @RowCnt = COUNT (ua.objectID)
    FROM    T_PMU_UserAccount ua
    WHERE   ua.personmasterID = @personMasterID
    AND     ua.transactTo = @transactToMax
    
    IF @RowCnt = 0
    BEGIN
        -- No current user account record was found, so no preferred record can exists => ie. delete any UA preferred record for this person
        DELETE
        FROM    T_PMU_UserAccount_Preferred
        WHERE   personMasterID = @personMasterID
    END
    ELSE
    BEGIN
        -- OK, a least ONE CURRENT user account record was found.
        
        -- Verify that a UAP record already exists, and if NOT =>
        IF NOT EXISTS (SELECT * FROM T_PMU_UserAccount_Preferred uap WHERE uap.personMasterID = @personMasterID)
        BEGIN
            -- create default preferred record (the one pointing to the oldest but CURRENT UA record.)
            EXEC spGK_PMU_InsertDefaultPreferred @personMasterID
        END
        ELSE
        BEGIN
            -- otherwise a preferred user account record exists, but it might point to a deleted user account
            
            IF NOT EXISTS
                (SELECT *
                FROM    T_PMU_UserAccount ua, T_PMU_UserAccount_Preferred uap
                WHERE   uap.personmasterID = @personMasterID
                AND     ua.objectID = uap.uaObjectID
                AND     ua.transactTo = @transactToMax
                )
            BEGIN
                -- And if that's the case - delete the preferred user account record, and create a new one...
                DELETE
                FROM    T_PMU_UserAccount_Preferred
                WHERE   personMasterID = @personMasterID
        
                -- ... and create default preferred user account...
                EXEC spGK_PMU_InsertDefaultPreferred @personMasterID
            END
        
        END
        
        -- In oder situations, the preferred user account should be kept pointing to present user account => ie. no change.
    END
    
LifeIsGood:
    SELECT  @aux = '',
            @RetVal = 0
    
ErrExit:
    IF @RetVal < 0
    BEGIN
        SELECT  @aux = 'It was not possible to map CPR number to loginname (in SP spGK_PMU_VerifyPreferredUA())! ' + @aux,
                @ErrorState = @RetVal * -1
        RAISERROR (@aux, @ErrorSeverity, @ErrorState)
    END

    RETURN @RetVal
END TRY

BEGIN CATCH
    IF @@trancount > 0 ROLLBACK TRANSACTION
    
    EXEC spGK_CORE_ErrorHandler @context
        
    RETURN @RetVal
END CATCH
go


CREATE PROCEDURE spGK_PMU_GetLoginNameFromCPR
    @context            VARCHAR(1020),
    @cprNo              VARCHAR(10),
    @loginName          VARCHAR(30)     OUTPUT,
    @aux                VARCHAR(1020)   OUTPUT
AS
BEGIN TRY
    SET NOCOUNT ON
    SET XACT_ABORT ON
    
    DECLARE @RowCnt                     INTEGER
    DECLARE @ErrNo                      INTEGER
    
    DECLARE @RetVal                     INTEGER
    SELECT  @RetVal         = -1
    
    DECLARE @ErrorSeverity              INTEGER
    SELECT  @ErrorSeverity  = 16
    DECLARE @ErrorState                 INTEGER
    
    -- Declare other parameters
    DECLARE @objectOwnerID              uniqueidentifier
    SELECT  @objectOwnerID  = NULL
    
    DECLARE @personMasterID             uniqueidentifier
    
    DECLARE @preferredLoginname         VARCHAR(30)
    
    DECLARE @transactToMax              DATETIME
    SELECT  @transactToMax = dbo.fnGK_core_DatetimeMaxValue()
    
    -- ---
    
    EXEC spGK_PMU_PrepareSPInvocation @context OUTPUT, spGK_PMU_GetLoginNameFromCPR, @cprNo
 
    -- Init parameters
    SELECT  @loginName = '',
            @aux = ''
    
    -- Validate parameters
    SET @RetVal = -2
    EXEC spGK_PM_GetObjectIDFromCPR @context, @cprNo, @objectOwnerID, @personMasterID OUTPUT, @aux OUTPUT
    
    IF @personMasterID IS NULL
    BEGIN
        SET @aux = 'CPR (' + @cprNo + ') was not found/could not be created. aux=[' + @aux + ']'
        GOTO ErrExit
    END
    
    SET @RetVal = -2
    
    -- Make sure that a preferred user account is in place
    EXEC spGK_PMU_VerifyPreferredUA @context, @personMasterID, @aux OUTPUT

    SELECT  @preferredLoginname = ua.loginName
    FROM    T_PMU_UserAccount ua, T_PMU_UserAccount_Preferred uap
    WHERE   ua.objectID = uap.uaObjectID
    AND     ua.transactTo = @transactToMax
    AND     uap.personMasterID = @personMasterID
    
    -- The empty string is returned in loginName OUTPUT parameter if no match was found
    IF @preferredLoginname IS NULL SET @preferredLoginname = ''

LifeIsGood:
    SELECT  @aux = '',
            @RetVal = 0,
            @loginName = @preferredLoginname
    
ErrExit:
    IF @RetVal < 0
    BEGIN
        SELECT  @aux = 'Loginname(s) could not be retrieved from CPR (in SP spGK_PMU_GetLoginNameFromCPR())! ' + @aux,
                @ErrorState = @RetVal * -1
        RAISERROR (@aux, @ErrorSeverity, @ErrorState)
    END

    -- Then select/return ALL rows
    SELECT      *
    FROM        T_PMU_UserAccount ua
    WHERE       ua.personMasterID = @personMasterID
    AND         ua.transactTo = @transactToMax
    ORDER BY    ua.transactFrom ASC
    
    SET @RowCnt = @@ROWCOUNT

    RETURN @RowCnt
END TRY

BEGIN CATCH
    IF @@trancount > 0 ROLLBACK TRANSACTION
    
    EXEC spGK_CORE_ErrorHandler @context
    
    SELECT @RetVal
    RETURN @RetVal
END CATCH
go


CREATE PROCEDURE spGK_PMU_LoginNameExist
    @context            VARCHAR(1020),
    @loginName          VARCHAR(30),
    @aux                VARCHAR(1020)       OUTPUT
AS
BEGIN TRY
    SET NOCOUNT ON
    SET XACT_ABORT ON
    
    DECLARE @RowCnt                     INTEGER
    DECLARE @ErrNo                      INTEGER
    
    DECLARE @RetVal                     INTEGER
    SELECT  @RetVal         = -1
    
    DECLARE @ErrorSeverity              INTEGER
    SELECT  @ErrorSeverity  = 16
    DECLARE @ErrorState                 INTEGER
    
    DECLARE @objectID                   uniqueidentifier
    DECLARE @personMasterID             uniqueidentifier
    
    DECLARE @transactFrom               DATETIME
    
    -- ---
    
    EXEC spGK_PMU_PrepareSPInvocation @context OUTPUT, spGK_PMU_LoginNameExist, @loginName
            
    -- Init parameters
    SELECT  @aux = ''
    
    EXEC @RowCnt = spGK_PMU_GetCurrentLoginRecord @context, @loginName, @objectID OUTPUT, @personMasterID OUTPUT, @transactFrom OUTPUT, @aux OUTPUT

LifeIsGood:
    SELECT  @aux = '',
            @RetVal = @RowCnt
        
ErrExit:
    IF @RetVal < 0
    BEGIN
        SELECT  @aux = 'It was not possible to determine if loginname exists (in SP spGK_PMU_LoginNameExist())!' + @aux,
                @ErrorState = @RetVal * -1
        RAISERROR (@aux, @ErrorSeverity, @ErrorState)
    END

    RETURN @RetVal
    
END TRY

BEGIN CATCH
    IF @@trancount > 0 ROLLBACK TRANSACTION
    
    EXEC spGK_CORE_ErrorHandler @context
    
    RETURN @RetVal
END CATCH
go


CREATE PROCEDURE spGK_PMU_RenameLoginName
    @context            VARCHAR(1020),
    @oldLoginName       VARCHAR(30),
    @newLoginName       VARCHAR(30),
    @aux                VARCHAR(1020)   OUTPUT
AS
BEGIN TRY
    SET NOCOUNT ON
    SET XACT_ABORT ON
    
    DECLARE @RowCnt                     INTEGER
    DECLARE @ErrNo                      INTEGER
    
    DECLARE @RetVal                     INTEGER
    SELECT  @RetVal         = -1
    
    DECLARE @ErrorSeverity              INTEGER
    SELECT  @ErrorSeverity  = 16
    DECLARE @ErrorState                 INTEGER
    
    DECLARE @uaObjectID                 uniqueidentifier
    DECLARE @personMasterIDRef          uniqueidentifier

    DECLARE @transactFrom               DATETIME
    DECLARE @transactNow                DATETIME
    DECLARE @transactToMax              DATETIME
    
    SELECT  @transactFrom   = GETDATE(),
            @transactNow    = GETDATE(),
            @transactToMax  = dbo.fnGK_core_DatetimeMaxValue()
            
    -- ---
    
    DECLARE @logText VARCHAR(2000)
    SELECT @logText = @oldLoginName + '==>' + @newLoginName
    
    EXEC spGK_PMU_PrepareSPInvocation @context OUTPUT, spGK_PMU_RenameLoginName, @logText
    
    -- Init parameters
    SELECT  @aux = ''
    
    -- Validate that old loginname is not empty OR NULL
    IF @oldLoginName IS NULL SET @oldLoginName = ''
    SET @oldLoginName = LTRIM( RTRIM(@oldLoginName) )
        
    IF @oldLoginName = ''
    BEGIN
        SET @aux = 'oldLoginName parameter is unspecified/empty. It''s pretty silly to attempt a login rename FROM nothing! '
        GOTO ErrExit
    END

    -- Validate that new loginname is not empty OR NULL
    SELECT  @RetVal = -2    
    
    IF @newLoginName IS NULL SET @newLoginName = ''
    SET @newLoginName = LTRIM( RTRIM(@newLoginName) )
    
    IF @newLoginName = ''
    BEGIN
        SET @aux = 'newLoginName parameter is unspecified/empty. It''s pretty silly to attempt a login rename TO nothing! '
        GOTO ErrExit
    END
    
    -- Login names are always stored in lowercase (by convention)
    SET @newLoginName = LOWER(@newLoginName)

    SELECT  @RetVal = -3

    -- Validate that the old login, actually exists
    EXEC @RowCnt = spGK_PMU_GetCurrentLoginRecord @context, @oldLoginName, @uaObjectID OUTPUT, @personMasterIDRef OUTPUT, @transactFrom OUTPUT, @aux OUTPUT

    IF @RowCnt <> 1
    BEGIN
        SET @aux = 'Loginname (' + @oldLoginName + ') was not found in the database! '
        GOTO ErrExit
    END
    
    BEGIN TRAN
    
        SELECT  @RetVal = -4
    
        -- First retire OLD login name record
        UPDATE  T_PMU_UserAccount
        SET     transactTo = @transactNow
        WHERE   objectId = @uaObjectID
        AND     transactTo = @transactToMax
        
        SELECT  @RowCnt = @@ROWCOUNT, @ErrNo = @@ERROR
        IF @RowCnt <> 1
        BEGIN
            SET @aux = 'UNKNOWN STATE: Old user account record could NOT be retired! '
            GOTO ErrExit
        END
        

        SELECT  @RetVal = -5
        
        -- Second, insert new current user login/account record
        INSERT INTO T_PMU_UserAccount VALUES ( @uaObjectID, @newLoginName, @personMasterIDRef, @transactNow, @transactToMax )

        SELECT  @RowCnt = @@ROWCOUNT, @ErrNo = @@ERROR
        IF @RowCnt <> 1
        BEGIN
            SET @aux = 'UNKNOWN STATE: New user account record could NOT be created! '
            GOTO ErrExit
        END
        
    COMMIT TRAN
    
LifeIsGood:
    SELECT  @aux = '',
            @RetVal = @RowCnt
        
ErrExit:
    IF @RetVal < 0
    BEGIN
        SELECT  @aux = 'Rename of loginname failed (in SP spGK_PMU_RenameLoginName()). ! ' + @aux,
                @ErrorState = @RetVal * -1
        RAISERROR (@aux, @ErrorSeverity, @ErrorState)
    END

    RETURN @RetVal    
END TRY

BEGIN CATCH
    IF @@trancount > 0 ROLLBACK TRANSACTION
    
    EXEC spGK_CORE_ErrorHandler @context
    
    RETURN @RetVal
END CATCH
go


CREATE PROCEDURE spGK_PMU_SetCPRLoginNameMap
    @context            VARCHAR(1020),
    @cprNo              VARCHAR(10),
    @loginName          VARCHAR(30),
    @aux                VARCHAR(1020)   OUTPUT
AS
BEGIN TRY
    SET NOCOUNT ON
    SET XACT_ABORT ON
    
    DECLARE @RowCnt                     INTEGER
    DECLARE @ErrNo                      INTEGER
    
    DECLARE @RetVal                     INTEGER
    SELECT  @RetVal = -1
    
    DECLARE @ErrorSeverity              INTEGER
    SELECT  @ErrorSeverity  = 16
    DECLARE @ErrorState                 INTEGER
    
    --DECLARE @package                    INTEGER
    --SELECT  @package        = 2 -- (PMU)
    
    DECLARE @objectOwnerID              uniqueidentifier
    SELECT  @objectOwnerID  = NULL
    
    DECLARE @personMasterID             uniqueidentifier

    DECLARE @uaObjectID                 uniqueidentifier
    DECLARE @personMasterIDRef          uniqueidentifier
    
    DECLARE @transactFrom               DATETIME
    DECLARE @transactNow                DATETIME
    DECLARE @transactToMax              DATETIME
    
    SELECT  @transactFrom   = GETDATE(),
            @transactNow    = GETDATE(),
            @transactToMax  = dbo.fnGK_core_DatetimeMaxValue()
            
    DECLARE @cprNoTry       VARCHAR(10)
    
    -- ---
    
    EXEC spGK_PMU_PrepareSPInvocation @context OUTPUT, spGK_PMU_SetCPRLoginNameMap, @cprNo
    
    -- Init relevant parameters
    IF @loginName IS NULL SET @loginName = ''
    SET @loginName = LTRIM( RTRIM(@loginName) )
    
    -- Login names are always stored in lowercase (by convention)
    SET @loginName = LOWER(@loginName)
    
    SET @aux = ''
    
    -- Validate parameters
    SET @RetVal = -2
    IF @loginName = ''
    BEGIN
        SET @aux = 'loginName parameter is unspecified/empty. '
        GOTO ErrExit
    END
    
    -- Transaction start point
    BEGIN TRAN

        SET @RetVal = -3
        EXEC spGK_PM_GetObjectIDFromCPR @context, @cprNo, @objectOwnerID, @personMasterID OUTPUT, @aux OUTPUT
    
        IF @personMasterID IS NULL
        BEGIN
            SET @aux = 'CPR (' + @cprNo + ') was not found/could not be created. aux=[' + @aux + ']'
            GOTO ErrExit
        END
        
        -- Condition so far:
        -- - We GOT a CPR number and a corresponding object ID
        -- - We MAY have an existing and current user account entry with the specified loginname => find out next ...
        
        EXEC @RowCnt = spGK_PMU_GetCurrentLoginRecord @context, @loginName, @uaObjectID OUTPUT, @personMasterIDRef OUTPUT, @transactFrom OUTPUT, @aux OUTPUT

        IF @RowCnt = 0
        BEGIN
            -- No current user account record was found, so one is created
            SET @RetVal = -5
            SET @uaObjectID = newid()
            INSERT INTO T_PMU_UserAccount VALUES ( @uaObjectID, @loginName, @personMasterID, @transactNow, @transactToMax )
        END
        ELSE
        BEGIN
            -- A current user account did actually exists. Now verfify that the record found, matches the @personMasterID found from the CPR number above.
            -- If NOT, it's a pretty unhealthy situation (think about this statement later??),
            -- indicating that the user account is already mapped to another CPR number, indicating some kind of misunderstanding.
            -- Flag the faulty condition as an error, but otherwise do nothing.
            SET @RetVal = -6
             
            IF @personMasterIDRef <> @personMasterID
            BEGIN
                -- Open key to be used for encrypting CPR
                OPEN SYMMETRIC KEY CprNoEncryptKey DECRYPTION BY CERTIFICATE CertForEncryptOfCprNoKey;
            
                -- Get the conflicting CPR number for insertion in error message
                SELECT  @cprNoTry = DecryptByKey(cpr.encryptedCprNo)
                FROM    T_PM_CPR cpr
                WHERE   cpr.personMasterID = @personMasterIDRef

                SET @aux = 'Login name (' + @loginName + ') is already mapped to CPR number (' + @cprNoTry + ') and can NOT be mapped to CPR (' + @cprNo + ') as requested. '
                GOTO ErrExit
            END
        END
    
    COMMIT TRAN
    
LifeIsGood:
    SELECT  @aux = '',
            @RetVal = 0
    
ErrExit:
    IF @RetVal < 0
    BEGIN
        SELECT  @aux = 'It was NOT possible to map CPR number to loginname (in SP spGK_PMU_SetCPRLoginNameMap())! ' + @aux,
                @ErrorState = @RetVal * -1
        RAISERROR (@aux, @ErrorSeverity, @ErrorState)
    END

    RETURN @RetVal
END TRY

BEGIN CATCH
    IF @@trancount > 0 ROLLBACK TRANSACTION
    
    EXEC spGK_CORE_ErrorHandler @context
    
    RETURN @RetVal
END CATCH
go


CREATE PROCEDURE spGK_PMU_SetPreferredUA
    @context            VARCHAR(1020),
    @loginName          VARCHAR(30),
    @aux                VARCHAR(1020)   OUTPUT
AS
BEGIN TRY
    SET NOCOUNT ON
    SET XACT_ABORT ON
    
    DECLARE @RowCnt                     INTEGER
    DECLARE @ErrNo                      INTEGER
    
    DECLARE @RetVal                     INTEGER
    SELECT  @RetVal         = -1
    
    DECLARE @ErrorSeverity              INTEGER
    SELECT  @ErrorSeverity  = 16
    DECLARE @ErrorState                 INTEGER

    DECLARE @uaObjectID                 uniqueidentifier
    DECLARE @personMasterIDRef          uniqueidentifier

    DECLARE @transactFrom               DATETIME
    DECLARE @transactNow                DATETIME
    DECLARE @transactToMax              DATETIME
    
    SELECT  @transactFrom   = GETDATE(),
            @transactNow    = GETDATE(),
            @transactToMax  = dbo.fnGK_core_DatetimeMaxValue()
    
    -- ---
    
    EXEC spGK_PMU_PrepareSPInvocation @context OUTPUT, spGK_PMU_SetPreferredUA, @loginName
    
    -- Init parameters
    SELECT  @aux = ''
    
    -- Validate that loginname is not empty OR NULL
    IF @loginName IS NULL SET @loginName = ''
    SET @loginName = LTRIM( RTRIM(@loginName) )
        
    IF @loginName = ''
    BEGIN
        SET @aux = 'loginName parameter is unspecified/empty! '
        GOTO ErrExit
    END
    
    SELECT  @RetVal = -2
    
    -- Validate that login actually exists
    EXEC @RowCnt = spGK_PMU_GetCurrentLoginRecord @context, @loginName, @uaObjectID OUTPUT, @personMasterIDRef OUTPUT, @transactFrom OUTPUT, @aux OUTPUT

    IF @RowCnt <> 1
    BEGIN
        SET @aux = 'Loginname (' + @loginName + ') was not found in the database! '
        GOTO ErrExit
    END
    
    BEGIN TRAN
    
        SELECT  @RetVal = -3
        DELETE
        FROM    T_PMU_UserAccount_Preferred
        WHERE   personMasterID = @personMasterIDRef
        
        SELECT  @RetVal = -4
        INSERT INTO T_PMU_UserAccount_Preferred VALUES (@personMasterIDRef, @uaObjectID)
        
        SELECT  @RowCnt = @@ROWCOUNT, @ErrNo = @@ERROR
    
    COMMIT TRAN
    
LifeIsGood:
    SELECT  @aux = '',
            @RetVal = @RowCnt
        
ErrExit:
    IF @RetVal < 0
    BEGIN
        SELECT  @aux = 'New preferred loginname could NOT be set (in SP spGK_PMU_SetPreferredUA())! ' + @aux,
                @ErrorState = @RetVal * -1
        RAISERROR (@aux, @ErrorSeverity, @ErrorState)
    END

    RETURN @RetVal
END TRY

BEGIN CATCH
    IF @@trancount > 0 ROLLBACK TRANSACTION
    
    EXEC spGK_CORE_ErrorHandler @context
        
    RETURN @RetVal
END CATCH
go


CREATE PROCEDURE spGK_PM_GetCPRFromObjectID
    @context            VARCHAR(1020),
    @objectID           uniqueidentifier,
    @cprNo              VARCHAR(10)         OUTPUT,
    @aux                VARCHAR(1020)       OUTPUT
AS
BEGIN TRY
    SET NOCOUNT ON
    SET XACT_ABORT ON
    
    DECLARE @RowCnt                     INTEGER
    DECLARE @ErrNo                      INTEGER
    
    DECLARE @RetVal                     INTEGER
    SELECT  @RetVal         = -1
    
    DECLARE @ErrorSeverity              INTEGER
    SELECT  @ErrorSeverity  = 16
    DECLARE @ErrorState                 INTEGER
    
    -- ---
    
    DECLARE @guidStr VARCHAR(38)
    SELECT @guidStr = dbo.fnGK_CORE_GetStringFromGUID(@objectID)
    
    EXEC spGK_PM_PrepareSPInvocation @context OUTPUT, spGK_PM_GetCPRFromObjectID, @guidStr
                    
    -- Prepare parameters
    SET @aux = ''
    
    -- Validates input
    -- NOT Done! Perhaps a TODO if needed...
    
    -- Open key to be used for decrypting CPR
    SET @RetVal = -2
    OPEN SYMMETRIC KEY CprNoEncryptKey DECRYPTION BY CERTIFICATE CertForEncryptOfCprNoKey;

    -- Get the CPR number that correspond to the specified objectID (if any). @cprNo is NULL if no match is found.
    SET @RetVal = -3
    SELECT  @cprNo = DecryptByKey(cpr.encryptedCprNo)
    FROM    T_PM_CPR cpr, T_PM_PersonMaster pm
    WHERE   cpr.personMasterID = pm.objectID
    AND     pm.objectID = @objectID
    
    IF @cprNo IS NULL SET @cprNo = ''
    
LifeIsGood:
    SELECT  @aux = '',
            @RetVal = 0
        
ErrExit:
    IF @RetVal < 0
    BEGIN
        SELECT  @aux = 'Retrieval of object ID FROM CPR failed! ' + @aux,
                @ErrorState = @RetVal * -1
        RAISERROR (@aux, @ErrorSeverity, @ErrorState)
    END

    RETURN @RetVal
END TRY

BEGIN CATCH
    IF @@trancount > 0 ROLLBACK TRANSACTION
    
    EXEC spGK_CORE_ErrorHandler @context
    
    RETURN @RetVal
END CATCH
go


-- Implementation of a procedure to get CPR numbers from an array of UUIDs.

CREATE PROCEDURE [dbo].[spGK_PM_GetCPRsFromObjectIDArray]
    @context            VARCHAR(1020),
    @objectIDArray      VARCHAR(MAX),
	@aux                VARCHAR(1020)       OUTPUT	
    
AS
BEGIN TRY
    SET NOCOUNT ON
    SET XACT_ABORT ON
    
    DECLARE @RowCnt                     INTEGER
    DECLARE @ErrNo                      INTEGER
    
    DECLARE @RetVal                     INTEGER
    SELECT  @RetVal                     = -1

	DECLARE @cprNo						varchar(max)
    
    DECLARE @ErrorSeverity              INTEGER
    SELECT  @ErrorSeverity              = 16
    DECLARE @ErrorState                 INTEGER    
    
	DECLARE @objectID                   VARCHAR(38)
	DECLARE @index			            INT
										-- In the return table we declare the UUID as a string and not a uniqueidentifier.
										-- The reason is that there is a risk that an exception could be thrown if an
										-- objectID is malformed - this is tested for in the the calling method, but
										-- just in case.
	DECLARE @ReturnTable                TABLE  (ID INT IDENTITY, CprNo VARCHAR(10), ObjectID VARCHAR(38), Aux VARCHAR(1020))
        
	-- ---
    
    EXEC spGK_PM_PrepareSPInvocation @context OUTPUT, spGK_PM_GetCPRsFromObjectIDArray, @objectIDArray
                    
    -- Open key to be used for decrypting CPR numbers
	SET @RetVal = -2
	OPEN SYMMETRIC KEY CprNoEncryptKey DECRYPTION BY CERTIFICATE CertForEncryptOfCprNoKey;
	
	SET @RetVal = -3
    -- Split the object IDs array and fetch the corresponding CPR numbers
    WHILE LEN(@objectIDArray) > 0
		BEGIN
			SET @RetVal = -4
			SET @aux = ''
			SET @index = CHARINDEX (',' , @objectIDArray)
			IF @INDEX > 0
				BEGIN
					SET @objectID = SUBSTRING(@objectIDArray , 1 , @index - 1)
					SET @objectIDArray = SUBSTRING(@objectIDArray , @index + 1 , LEN(@objectIDArray) - @index)
				END
			ELSE
				BEGIN
					SET @objectID = @objectIDArray
					SET @objectIDArray = ''
				END
		
			-- Get the CPR number that corresponds to the current objectID. @cprNo is NULL if no match is found.

			IF LEN(@objectID) > 0 -- We abandon null values
				BEGIN
					SET @RetVal = -5
					SELECT  @cprNo = cpr.encryptedCprNo
					FROM    T_PM_CPR cpr, T_PM_PersonMaster pm
					WHERE   cpr.personMasterID = pm.objectID
					AND     pm.objectID = @objectID
    
					SET @RetVal = -6
					IF @cprNo IS NULL
						BEGIN
							SET @RetVal = -7
							INSERT INTO @ReturnTable (CprNo, ObjectID, Aux) VALUES ('', @objectID, 'Retrieval of object ID FROM CPR failed!')
						END
					ELSE
						BEGIN
							SET @RetVal = -8
							INSERT INTO @ReturnTable (CprNo, ObjectID, Aux) VALUES (DecryptByKey(@cprNo), @objectID, @aux)
						END
				END
			ELSE -- We treat null values as errors
				BEGIN
					SET @RetVal = -9
					INSERT INTO @ReturnTable (CprNo, ObjectID, Aux) VALUES (NULL, @objectID, 'Retrieval of object ID FROM CPR failed!\nReason: a null value was given as object ID.')
				END
		END

		-- update @ReturnTable set aux  = "..."	

	LifeIsGood:
		SELECT  @aux = '',
				@RetVal = 0
        
	ErrExit:
		IF @RetVal < 0
		BEGIN
			SELECT  @aux = 'Retrieval of object ID FROM CPR failed! ' + @aux,
					@ErrorState = @RetVal * -1
			RAISERROR (@aux, @ErrorSeverity, @ErrorState)
		END
	
	SELECT * FROM @ReturnTable
	RETURN @RetVal
END TRY

BEGIN CATCH
    IF @@trancount > 0 ROLLBACK TRANSACTION
    
    EXEC spGK_CORE_ErrorHandler @context
    
    RETURN @RetVal
END CATCH

GO


CREATE PROCEDURE spGK_PM_InitDB
AS
BEGIN
    SET NOCOUNT ON

    -- First create DB master key
    IF NOT EXISTS 
    (SELECT * FROM sys.symmetric_keys WHERE symmetric_key_id = 101)
    CREATE MASTER KEY ENCRYPTION BY 
    PASSWORD = '<pm-cryptpassword>'
    
    -- Second, create certificate to protect encryption keys
    IF NOT EXISTS 
    (SELECT * FROM sys.certificates WHERE name = 'CertForEncryptOfCprNoKey')
    CREATE CERTIFICATE CertForEncryptOfCprNoKey
    WITH SUBJECT = 'certificate_for_the_encryption_of_cprno',
    START_DATE   = '20100101',
    EXPIRY_DATE  = '21000101'
    
    -- Third, create the key that will be used for the encryption of CPR numbers
    IF NOT EXISTS 
    (SELECT * FROM sys.symmetric_keys WHERE name = 'CprNoEncryptKey')
    CREATE SYMMETRIC KEY CprNoEncryptKey WITH ALGORITHM = AES_128
    ENCRYPTION BY CERTIFICATE CertForEncryptOfCprNoKey
    
    -- Initialize object owner for object created by this enterprice
    DECLARE @objectOwnerID      uniqueidentifier
    
    DECLARE @namespace          VARCHAR(510)
    SELECT  @namespace = dbo.fnGK_CORE_GetConfigValue('namespace-self')
    
    DECLARE @aux                VARCHAR(1020)
    SELECT  @aux = 'CREATE-ON-NON-EXISTENCE'
    
    EXEC spGK_PM_GetOwnerIDFromNamespace 'Called from spGK_PM_InitDB', @objectOwnerID, @namespace, @aux

    SET NOCOUNT OFF
END
go

EXEC spGK_PM_InitDB
GO


CREATE PROCEDURE spGK_PM_Probe
    @context        VARCHAR(1020),
    @aux            VARCHAR(1020)   OUTPUT
AS
BEGIN
    DECLARE @RowCnt INTEGER
    
    SELECT @aux = '<db-version>' + dbo.fnGK_CORE_GetConfigValue('db-version') + '</db-version>'
    
    SELECT  @RowCnt = COUNT (pm.objectID)
    FROM    T_PM_PersonMaster pm
    
    RETURN @RowCnt
END
go

EXEC [dbo].[spGK_PM_InitDB]
go