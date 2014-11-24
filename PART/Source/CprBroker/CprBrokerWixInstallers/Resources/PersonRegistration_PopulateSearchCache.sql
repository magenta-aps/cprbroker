-- =============================================
-- Author:		Beemen Beshara
-- Description:	Trigger for changes in PersonRegistration, 
--  refreshes the cached serach table by calling InitializePersonSearchCache for 
--  each record being inserted or updated
-- =============================================

IF EXISTS (SELECT * FROM sys.triggers where name='PersonRegistration_PopulateSearchCache')
BEGIN
	DROP TRIGGER dbo.PersonRegistration_PopulateSearchCache
END
GO

CREATE TRIGGER dbo.PersonRegistration_PopulateSearchCache
   ON  dbo.PersonRegistration
   AFTER INSERT,UPDATE
AS 
BEGIN
	--SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @UUID UNIQUEIDENTIFIER, @PersonRegistrationId UNIQUEIDENTIFIER, @RegistrationDate DATETIME, @Contents XML
	
	DECLARE CUR Cursor FOR
	SELECT UUID, PersonRegistrationId, RegistrationDate , Contents
	FROM    INSERTED
	
	OPEN CUR
	FETCH NEXT FROM CUR INTO @UUID, @PersonRegistrationId, @RegistrationDate, @Contents
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
        -- Do not allow errors (i.e. conversion to bool or datetime) to break the data update
        BEGIN TRY
		    EXEC dbo.InitializePersonSearchCache @UUID, @PersonRegistrationId, @RegistrationDate, @Contents
        END TRY
        BEGIN CATCH
            
        END CATCH
        
        -- Fetch the next row anyway
        FETCH NEXT FROM CUR INTO @UUID, @PersonRegistrationId, @RegistrationDate, @Contents
	END
	
	CLOSE CUR
	DEALLOCATE CUR

END
GO



