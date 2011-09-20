/*==============================================================*/
/* DBMS name:      Microsoft SQL Server 2008                    */
/* Created on:     10-03-2011 11:42:08                          */
/*==============================================================*/



if exists (select 1
          from sysobjects
          where  id = object_id('spGK_PM_GetObjectIDsFromCPRArray')
          and type in ('P','PC'))
   drop procedure spGK_PM_GetObjectIDsFromCPRArray
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
			AND DecryptByKey(cpr.encryptedCprNo) = RET.CprNo
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
		SELECT EncryptByKey(key_GUID('CprNoEncryptKey'), CprNo), Birthdate, Gender, ObjectID, GETDATE()
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

