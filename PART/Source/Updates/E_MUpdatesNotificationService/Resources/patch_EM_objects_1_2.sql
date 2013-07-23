
/*======================================================================
 =======================================================================
	Patch the database to avoid tracking of AA70300T (children) table
 =======================================================================
=======================================================================*/

/*************************************************************************
	Create table T_E_MUpdateStaging_Excluded
*************************************************************************/

IF NOT EXISTS (SELECT * FROM sysobjects where name='T_E_MUpdateStaging_Excluded')
	CREATE TABLE dbo.T_E_MUpdateStaging_Excluded
	(
		Id int NOT NULL IDENTITY (1, 1),
		PERSONNUMMER decimal(11, 0) NOT NULL,
		E_MTable varchar(120) NOT NULL,
		CreateTS datetime NOT NULL
	)  ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_T_E_MUpdateStaging_Excluded')
	ALTER TABLE dbo.T_E_MUpdateStaging_Excluded ADD CONSTRAINT
	PK_T_E_MUpdateStaging_Excluded PRIMARY KEY CLUSTERED 	(Id) 
	WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
	ON [PRIMARY]
GO

/*************************************************************************
	Fill T_E_MUpdateStaging_Excluded, and delete from T_E_MUpdateStaging
*************************************************************************/

INSERT INTO T_E_MUpdateStaging_Excluded (PERSONNUMMER, E_MTable, CreateTS) 
SELECT S.PERSONNUMMER, S.E_MTable, S.CreateTS 
FROM T_E_MUpdateStaging S
LEFT OUTER JOIN AA70000T P ON P.PERSONNUMMER = S.PERSONNUMMER
WHERE P.PERSONNUMMER IS NULL

GO

DELETE S
FROM T_E_MUpdateStaging S
WHERE EXISTS (SELECT * FROM T_E_MUpdateStaging_Excluded SE WHERE SE.PERSONNUMMER = S.PERSONNUMMER)

GO

/*************************************************************************
						THE DIFF PROCEDURE
	This is the procedure called after a KMD synchronizations is
	done.
	This is where our comparison takes place.
*************************************************************************/

IF EXISTS (SELECT * FROM sys.procedures where name = 'usp_CPR_Broker_Compare')
	DROP PROCEDURE [dbo].[usp_CPR_Broker_Compare]
GO

CREATE PROCEDURE [dbo].[usp_CPR_Broker_Compare] AS
	BEGIN
		SET NOCOUNT ON
		INSERT INTO AA70000T_TMP2 SELECT * FROM AA70000T;
		INSERT INTO AA70300T_TMP2 SELECT * FROM AA70300T;
        EXEC xp_logevent 70000, 'usp_CPR_Broker_Compare is starting...', informational

		DECLARE @T_E_MUpdateStaging_Tmp Table(PERSONNUMMER DECIMAL(11,0), E_MTable VARCHAR(120), CreateTS DateTime, Valid BIT)

		-- We check if the temp tables actually contains data to avoid mistakedly updates --				
		IF (EXISTS (SELECT PERSONNUMMER FROM AA70000T_TMP1) AND EXISTS (SELECT PERSONNUMMER FROM AA70000T_TMP2))
            BEGIN			
				EXEC xp_logevent 70000, '... both AA70000 tables contain data...', informational
				
				-- We look for updates in already existing records --
				INSERT INTO @T_E_MUpdateStaging_Tmp (PERSONNUMMER, E_MTable, CreateTS, Valid)
					SELECT PERSONNUMMER, 'AA70000T', current_timestamp, 0
					FROM (
						SELECT PERSONNUMMER, COUNT(*) AS PNR_COUNT  FROM (
							SELECT * FROM AA70000T_TMP1 
								UNION 
							SELECT * FROM AA70000T_TMP2
						) AS un
						GROUP BY PERSONNUMMER
					) AS Counts 
					WHERE PNR_COUNT > 1
                EXEC xp_logevent 70000, '... checked for differences...', informational
				-- We look for new records --
				INSERT INTO @T_E_MUpdateStaging_Tmp (PERSONNUMMER, E_MTable, CreateTS, Valid)
					SELECT t2.PERSONNUMMER, 'AA70000T', current_timestamp, 0 
					FROM AA70000T_TMP2 t2 LEFT OUTER JOIN AA70000T_TMP1 t1
					ON t2.PERSONNUMMER = t1.PERSONNUMMER
					WHERE t1.PERSONNUMMER is null
                EXEC xp_logevent 70000, '... checked for new records...', informational
			END
		
		-- Only if both temp tables contains data we carry out the comparison --
		IF (EXISTS (SELECT PERSONNUMMER FROM AA70300T_TMP1) AND EXISTS (SELECT PERSONNUMMER FROM AA70300T_TMP2))
            BEGIN
				EXEC xp_logevent 70000, '... both AA70300 tables contain data...', informational
				
				-- We look for updates in already existing records --
				INSERT INTO @T_E_MUpdateStaging_Tmp (PERSONNUMMER, E_MTable, CreateTS, Valid)
					SELECT PERSONNUMMER, 'AA70300T', current_timestamp, 0
					FROM (
						SELECT PERSONNUMMER, COUNT(*) AS PNR_COUNT  FROM (
							SELECT * FROM AA70300T_TMP1 
								UNION 
							SELECT * FROM AA70300T_TMP2
						) AS un
						GROUP BY PERSONNUMMER
					) AS Counts 
					WHERE PNR_COUNT > 1
                EXEC xp_logevent 70000, '... checked for differences...', informational
				-- We look for new records --
				INSERT INTO @T_E_MUpdateStaging_Tmp (PERSONNUMMER, E_MTable, CreateTS, Valid)
					SELECT t2.PERSONNUMMER, 'AA70300T', current_timestamp, 0
					FROM AA70300T_TMP2 t2 LEFT OUTER JOIN AA70300T_TMP1 t1
					ON t2.PERSONNUMMER = t1.PERSONNUMMER
					WHERE t1.PERSONNUMMER is null
                EXEC xp_logevent 70000, '... checked for new records...', informational
			END

		
		-- Mark the validity of the records
		UPDATE T
		SET Valid = 1
		FROM @T_E_MUpdateStaging_Tmp T
		INNER JOIN AA70000T P ON T.PERSONNUMMER = P.PERSONNUMMER
		
		-- Now insert either in staging table or excluded staging table
		INSERT INTO T_E_MUpdateStaging (PERSONNUMMER, E_MTable, CreateTS)
		SELECT PERSONNUMMER, E_MTable, CreateTS
		FROM @T_E_MUpdateStaging_Tmp
		WHERE Valid = 1

		INSERT INTO T_E_MUpdateStaging_Excluded (PERSONNUMMER, E_MTable, CreateTS)
		SELECT PERSONNUMMER, E_MTable, CreateTS
		FROM @T_E_MUpdateStaging_Tmp
		WHERE Valid = 0

		-- Finally we truncate all the temp tables --
		TRUNCATE TABLE AA70000T_TMP1;
		TRUNCATE TABLE AA70300T_TMP1;
		TRUNCATE TABLE AA70000T_TMP2;
		TRUNCATE TABLE AA70300T_TMP2;
        EXEC xp_logevent 70000, '... usp_CPR_Broker_Compare finished.', informational
	END
GO
