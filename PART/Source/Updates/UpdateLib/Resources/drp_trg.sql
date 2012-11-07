/*==============================================================*/
/* DBMS name:      Microsoft SQL Server 2005                    */
/* Created on:     23-03-2011 07:18:47                          */
/*==============================================================*/


-- Drop all existing triggers regardless of _<SystemName>Table content
    
DECLARE @triggerNamePrefix NVARCHAR(120)
SELECT @triggerNamePrefix = dbo.fnGK_<SystemName>UpdateStaging_Get_TriggerNamePrefix() + '%'

DECLARE @trgName NVARCHAR(120)

DECLARE TrgName_Cursor CURSOR FOR SELECT name FROM sysobjects WHERE name LIKE @triggerNamePrefix AND type = 'TR'

OPEN TrgName_Cursor

FETCH FROM TrgName_Cursor INTO @trgName

WHILE @@FETCH_STATUS = 0
BEGIN
    --PRINT @trgName
        
    EXECUTE spGK_<SystemName>UpdateStaging_Drop_Trigger @trgName

    FETCH NEXT FROM TrgName_Cursor INTO @trgName
END
    
CLOSE TrgName_Cursor
DEALLOCATE TrgName_Cursor

GO


-- Now drop the stored procedures

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

