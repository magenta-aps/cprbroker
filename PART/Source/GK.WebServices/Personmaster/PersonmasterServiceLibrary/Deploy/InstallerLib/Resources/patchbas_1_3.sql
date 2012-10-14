
-- Implementation of a procedure to get CPR numbers from an array of UUIDs.
if exists (select 1
          from sysobjects
          where  id = object_id('spGK_PM_GetCPRsFromObjectIDArray')
          and type in ('P','PC'))
   drop procedure spGK_PM_GetCPRsFromObjectIDArray

GO


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



