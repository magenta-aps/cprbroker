-- =============================================
-- Author:		Beemen Beshara
-- Description:	Trigger for record deletion in PersonRegistration, 
--  Deleted the associated cache records if no other records are available
-- =======================================================================

IF EXISTS (SELECT * FROM sys.triggers where name='PersonRegistration_DeleteSearchCache')
BEGIN
	DROP TRIGGER dbo.PersonRegistration_DeleteSearchCache
END
GO

CREATE TRIGGER dbo.PersonRegistration_DeleteSearchCache
   ON  dbo.PersonRegistration
   AFTER DELETE
AS 
BEGIN
	--SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
	SET NOCOUNT ON;
	
	BEGIN TRY
		DELETE PSC
		FROM PersonSearchCache PSC
		INNER JOIN DELETED D ON D.UUID = PSC.UUID AND D.PersonRegistrationId = PSC.PersonRegistrationId			
    END TRY
    BEGIN CATCH
		-- Do nothing            
    END CATCH

	DECLARE @UUID UNIQUEIDENTIFIER, @PersonRegistrationId UNIQUEIDENTIFIER, @RegistrationDate DATETIME, @Contents XML
	DECLARE CUR CURSOR FOR SELECT DISTINCT UUID FROM DELETED
	DECLARE @PersonRegistrationId_Cache UNIQUEIDENTIFIER

	OPEN CUR
	FETCH NEXT FROM CUR INTO @UUID

	WHILE @@FETCH_STATUS = 0
	BEGIN
		SELECT TOP (1) @PersonRegistrationId=PersonRegistrationId, @RegistrationDate=RegistrationDate, @Contents=Contents
		FROM PersonRegistration PR
		WHERE UUID = @UUID AND PersonRegistrationId NOT IN (SELECT PersonRegistrationId FROM DELETED WHERE UUID=@UUID)
		ORDER BY RegistrationDate DESC, BrokerUpdateDate DESC

		IF @PersonRegistrationId IS NOT NULL -- If there are still records for this person
		BEGIN
			EXEC dbo.InitializePersonSearchCache @UUID, @PersonRegistrationId, @RegistrationDate, @Contents
		END

		FETCH NEXT FROM CUR INTO @UUID
	END

	CLOSE CUR
	DEALLOCATE CUR
END
GO
