if exists (select 1
          from sysobjects
          where  id = object_id('fnGK_DPRUpdateStaging_Get_TriggerNamePrefix')
          and type in ('IF', 'FN', 'TF'))
   drop function fnGK_DPRUpdateStaging_Get_TriggerNamePrefix
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


IF NOT EXISTS (SELECT * FROM T_DPRUpdateStaging_DPRTable WHERE DPRTable = 'DTPERSBO')
	INSERT INTO T_DPRUpdateStaging_DPRTable (DPRTable) VALUES ('DTPERSBO')

GO

EXECUTE spGK_DPRUpdateStaging_Create_Trigger 'DTPERSBO'
GO

-- Optional

INSERT INTO T_DPRUpdateStaging (PNR, DPRTable, CreateTS)
SELECT DISTINCT PNR, 'DTPERSBO', GETDATE()
FROM DTPERSBO
WHERE TFDTOMRK = 'U'

GO
