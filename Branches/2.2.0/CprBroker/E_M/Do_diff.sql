IF EXISTS (SELECT * FROM sysobjects where name='AA70000T_TMP1')
	DROP TABLE AA70000T_TMP1
IF EXISTS (SELECT * FROM sysobjects where name='AA70300T_TMP1')
	DROP TABLE AA70300T_TMP1
IF EXISTS (SELECT * FROM sysobjects where name='AA70000T_TMP2')
	DROP TABLE AA70000T_TMP2
IF EXISTS (SELECT * FROM sysobjects where name='AA70300T_TMP2')
	DROP TABLE AA70300T_TMP2
IF EXISTS (SELECT * FROM sysobjects where name='usp_CPR_Broker_Prepare')
	DROP PROCEDURE usp_CPR_Broker_Prepare
IF EXISTS (SELECT * FROM sysobjects where name='usp_CPR_Broker_Compare')
	DROP PROCEDURE usp_CPR_Broker_Compare
GO

SELECT TOP 0 * INTO AA70000T_TMP1 FROM AA70000T;
SELECT TOP 0 * INTO AA70300T_TMP1 FROM AA70300T;

SELECT TOP 0 * INTO AA70000T_TMP2 FROM AA70000T;
SELECT TOP 0 * INTO AA70300T_TMP2 FROM AA70300T;


truncate table AA70000T_TMP1
truncate table AA70300T_TMP1
truncate table AA70000T_TMP2
truncate table AA70300T_TMP2
GO

/*************************************************************************
						THE PREPARE PROCEDURE
	This is the procedure called before a KMD synchronizations is
	initiated.
*************************************************************************/

CREATE PROCEDURE [dbo].[usp_CPR_Broker_Prepare] AS
	BEGIN
		SET NOCOUNT ON
		INSERT INTO AA70000T_TMP1 SELECT * FROM AA70000T;
		INSERT INTO AA70300T_TMP1 SELECT * FROM AA70300T;
	END
GO

/*************************************************************************
						THE DIFF PROCEDURE
	This is the procedure called after a KMD synchronizations is
	done.
	This is where our comparison takes place.
*************************************************************************/

CREATE PROCEDURE [dbo].[usp_CPR_Broker_Compare] AS
	BEGIN
		SET NOCOUNT ON
		INSERT INTO AA70000T_TMP2 SELECT * FROM AA70000T;
		INSERT INTO AA70300T_TMP2 SELECT * FROM AA70300T;

		IF (EXISTS (SELECT PERSONNUMMER FROM AA70000T_TMP1) AND EXISTS (SELECT PERSONNUMMER FROM AA70000T_TMP2))
			-- We check if the temp tables actually contains data to avoid mistakedly updates --
			BEGIN
				-- We look for updates in already existing records --
				INSERT INTO T_E_MUpdateStaging (PERSONNUMMER, E_MTable, CreateTS) 
					SELECT PERSONNUMMER, 'AA70000T', current_timestamp
					FROM (
						SELECT PERSONNUMMER, COUNT(*) AS PNR_COUNT  FROM (
							SELECT * FROM AA70000T_TMP1 
								UNION 
							SELECT * FROM AA70000T_TMP2
						) AS un
						GROUP BY PERSONNUMMER
					) AS Counts 
					WHERE PNR_COUNT > 1
				-- We look for new records --
				INSERT INTO T_E_MUpdateStaging (PERSONNUMMER, E_MTable, CreateTS) 
					SELECT t2.PERSONNUMMER, 'AA70000T', current_timestamp 
					FROM AA70000T_TMP2 t2 LEFT OUTER JOIN AA70000T_TMP1 t1
					ON t2.PERSONNUMMER = t1.PERSONNUMMER
					WHERE t1.PERSONNUMMER is null
			END
		IF (EXISTS (SELECT PERSONNUMMER FROM AA70300T_TMP1) AND EXISTS (SELECT PERSONNUMMER FROM AA70300T_TMP2))
			-- Only if both temp tables contains data we carry out the comparison --
			BEGIN
				-- We look for updates in already existing records --
				INSERT INTO T_E_MUpdateStaging (PERSONNUMMER, E_MTable, CreateTS) 
					SELECT PERSONNUMMER, 'AA70300T', current_timestamp
					FROM (
						SELECT PERSONNUMMER, COUNT(*) AS PNR_COUNT  FROM (
							SELECT * FROM AA70300T_TMP1 
								UNION 
							SELECT * FROM AA70300T_TMP2
						) AS un
						GROUP BY PERSONNUMMER
					) AS Counts 
					WHERE PNR_COUNT > 1
				-- We look for new records --
				INSERT INTO T_E_MUpdateStaging (PERSONNUMMER, E_MTable, CreateTS) 
					SELECT t2.PERSONNUMMER, 'AA70300T', current_timestamp 
					FROM AA70300T_TMP2 t2 LEFT OUTER JOIN AA70300T_TMP1 t1
					ON t2.PERSONNUMMER = t1.PERSONNUMMER
					WHERE t1.PERSONNUMMER is null
			END
		-- Finally we truncate all the temp tables --
		TRUNCATE TABLE AA70000T_TMP1;
		TRUNCATE TABLE AA70300T_TMP1;
		TRUNCATE TABLE AA70000T_TMP2;
		TRUNCATE TABLE AA70300T_TMP2;
	END
GO