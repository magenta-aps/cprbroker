/*==============================================================*/
/* DBMS name:      Microsoft SQL Server 2005                    */
/* Created on:     23-03-2011 07:18:47                          */
/*==============================================================*/


if exists (select 1
          from sysobjects
          where  id = object_id('fnGK_<SystemName>UpdateStaging_Get_TriggerNamePrefix')
          and type in ('IF', 'FN', 'TF'))
   drop function fnGK_<SystemName>UpdateStaging_Get_TriggerNamePrefix
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_<SystemName>UpdateStaging_ResetAllTriggers')
          and type in ('P','PC'))
   drop procedure spGK_<SystemName>UpdateStaging_ResetAllTriggers
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_<SystemName>UpdateStaging_Create_Trigger')
          and type in ('P','PC'))
   drop procedure spGK_<SystemName>UpdateStaging_Create_Trigger
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_<SystemName>UpdateStaging_Drop_Trigger')
          and type in ('P','PC'))
   drop procedure spGK_<SystemName>UpdateStaging_Drop_Trigger
go

if exists (select 1
            from  sysobjects
           where  id = object_id('<StagingTableName>')
            and   type = 'U')
   drop table <StagingTableName>
go

if exists (select 1
            from  sysobjects
           where  id = object_id('<TablesTableName>')
            and   type = 'U')
   drop table <TablesTableName>
go
