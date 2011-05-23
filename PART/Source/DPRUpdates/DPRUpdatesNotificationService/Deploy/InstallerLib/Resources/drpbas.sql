/*==============================================================*/
/* DBMS name:      Microsoft SQL Server 2005                    */
/* Created on:     23-03-2011 07:18:47                          */
/*==============================================================*/


if exists (select 1
          from sysobjects
          where  id = object_id('fnGK_DPRUpdateStaging_Get_TriggerNamePrefix')
          and type in ('IF', 'FN', 'TF'))
   drop function fnGK_DPRUpdateStaging_Get_TriggerNamePrefix
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_DPRUpdateStaging_ResetAllTriggers')
          and type in ('P','PC'))
   drop procedure spGK_DPRUpdateStaging_ResetAllTriggers
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_DPRUpdateStaging_Create_Trigger')
          and type in ('P','PC'))
   drop procedure spGK_DPRUpdateStaging_Create_Trigger
go

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_DPRUpdateStaging_Drop_Trigger')
          and type in ('P','PC'))
   drop procedure spGK_DPRUpdateStaging_Drop_Trigger
go

if exists (select 1
            from  sysobjects
           where  id = object_id('T_DPRUpdateStaging')
            and   type = 'U')
   drop table T_DPRUpdateStaging
go

if exists (select 1
            from  sysobjects
           where  id = object_id('T_DPRUpdateStaging_DPRTable')
            and   type = 'U')
   drop table T_DPRUpdateStaging_DPRTable
go
