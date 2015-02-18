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


CREATE FUNCTION fnGK_DPRUpdateStaging_Get_TriggerNamePrefix ()
RETURNS NVARCHAR(120)
BEGIN
    RETURN 'trgGK_DPRUpdateStaging_'
END
go


CREATE PROCEDURE spGK_DPRUpdateStaging_Drop_Trigger
    @triggerName  NVARCHAR(120)
AS
    SET NOCOUNT ON

    DECLARE @sqlStmt NVARCHAR(1020)
    
    IF @triggerName IS NULL SET @triggerName = ''
    SET @triggerName = LTRIM( RTRIM( @triggerName ) )
    
    IF @triggerName = '' RETURN
    
    IF CHARINDEX(dbo.fnGK_DPRUpdateStaging_Get_TriggerNamePrefix(), @triggerName) = 0 RETURN
    
    -- Delete trigger if it exists
    SELECT @sqlStmt =
'
IF OBJECT_ID ( ''' + @triggerName + ''', ''TR'' ) IS NOT NULL DROP TRIGGER ' + @triggerName + '
'
    EXECUTE sp_executesql @sqlStmt
    
    SET NOCOUNT OFF
go


CREATE PROCEDURE spGK_DPRUpdateStaging_Create_Trigger
    @tableName  NVARCHAR(120)
AS
    SET NOCOUNT ON

    DECLARE @triggerName NVARCHAR(120)
    DECLARE @sqlStmt NVARCHAR(1020)
    
    IF @tableName IS NULL SET @tableName = ''
    SET @tableName = UPPER( LTRIM( RTRIM( @tableName ) ) )
    
    IF @tableName = '' RETURN
    
    SELECT @triggerName = dbo.fnGK_DPRUpdateStaging_Get_TriggerNamePrefix() + @tableName

    EXECUTE spGK_DPRUpdateStaging_Drop_Trigger @triggerName

    -- Create trigger
    SELECT @sqlStmt =
'
CREATE TRIGGER ' + @triggerName + '
   ON ' + @tableName + '
   AFTER INSERT,UPDATE
AS 
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

		INSERT INTO T_DPRUpdateStaging (PNR, DPRTable, CreateTS)
		SELECT PNR, ''' + @tableName + ''', GETDATE()
		FROM inserted

	END TRY

	BEGIN CATCH
		-- Just ignore any errors
	END CATCH

	SET NOCOUNT OFF
END
'
    EXECUTE sp_executesql @sqlStmt

    SET NOCOUNT OFF
go


CREATE PROCEDURE spGK_DPRUpdateStaging_ResetAllTriggers
    @deleteOnly INTEGER
AS
    SET NOCOUNT ON

    DECLARE @triggerNamePrefix NVARCHAR(120)
    SELECT @triggerNamePrefix = dbo.fnGK_DPRUpdateStaging_Get_TriggerNamePrefix() + '%'
    
    
    -- Drop all existing triggers regardless of _DPRTable content
    
    DECLARE @trgName NVARCHAR(120)
            
    DECLARE TrgName_Cursor CURSOR FOR SELECT name FROM sysobjects WHERE name LIKE @triggerNamePrefix AND type = 'TR'
    
    OPEN TrgName_Cursor
    
    FETCH FROM TrgName_Cursor INTO @trgName
    WHILE @@FETCH_STATUS = 0
    BEGIN
        --PRINT @trgName
        
        EXECUTE spGK_DPRUpdateStaging_Drop_Trigger @trgName

        FETCH NEXT FROM TrgName_Cursor INTO @trgName
    END
    
    CLOSE TrgName_Cursor
    DEALLOCATE TrgName_Cursor

    -- If @deleteOnly is NULL on SP call - new trigges will NOT be created
    IF @deleteOnly IS NULL SET @deleteOnly = 1
    
    -- Skip creation of new triggers
    IF @deleteOnly <> 0 RETURN
    
    -- Create triggers on all tables specified in T_DPRUpdateStaging_DPRTable table

    DECLARE @tblName NVARCHAR(120)
            
    DECLARE TblName_Cursor CURSOR FOR SELECT DPRTable FROM T_DPRUpdateStaging_DPRTable
    
    OPEN TblName_Cursor
    
    FETCH FROM TblName_Cursor INTO @tblName
    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- PRINT @tblName
        
        EXECUTE spGK_DPRUpdateStaging_Create_Trigger @tblName

        FETCH NEXT FROM TblName_Cursor INTO @tblName
    END
    
    CLOSE TblName_Cursor
    DEALLOCATE TblName_Cursor
   
    SET NOCOUNT OFF
go

EXEC spGK_DPRUpdateStaging_ResetAllTriggers 0

go