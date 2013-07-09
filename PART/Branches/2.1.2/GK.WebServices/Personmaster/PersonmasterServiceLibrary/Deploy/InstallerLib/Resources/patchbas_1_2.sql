-- PATCH
/*==============================================================*/
/* DBMS name:      Microsoft SQL Server 2008                    */
/* Created on:     10-03-2011 11:42:08                          */
/*==============================================================*/


if not exists (select * from sys.columns c inner join sys.tables t on c.object_id = t.object_id where t.object_id=object_id('T_PM_CPR') and c.name = 'cprNo')
	ALTER TABLE dbo.T_PM_CPR ADD
		cprNo varchar(10) NOT NULL CONSTRAINT DF_T_PM_CPR_cprNo DEFAULT '0000000000'

GO

if not exists (SELECT * from sys.indexes where name = 'IX_T_PM_CPR_CprNo')
	CREATE NONCLUSTERED INDEX IX_T_PM_CPR_CprNo ON dbo.T_PM_CPR
		(
		cprNo
		) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO


OPEN SYMMETRIC KEY CprNoEncryptKey DECRYPTION BY CERTIFICATE CertForEncryptOfCprNoKey;

UPDATE T_PM_CPR 
SET cprNo = DecryptByKey(encryptedCprNo) WHERE cprNo = '0000000000'

GO

if exists (select * from sys.default_constraints where name = 'DF_T_PM_CPR_cprNo')
	ALTER TABLE T_PM_CPR DROP CONSTRAINT DF_T_PM_CPR_cprNo

GO

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_PM_GetObjectIDsFromCPRArray')
          and type in ('P','PC'))
   drop procedure spGK_PM_GetObjectIDsFromCPRArray

GO

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

GO


if exists (select 1
          from sysobjects
          where  id = object_id('spGK_PM_GetObjectIDFromCPR')
          and type in ('P','PC'))
   drop procedure spGK_PM_GetObjectIDFromCPR

GO

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

GO

if exists (select 1
          from sysobjects
          where  id = object_id('spGK_PMU_GetCPRFromLoginName')
          and type in ('P','PC'))
   drop procedure spGK_PMU_GetCPRFromLoginName
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

