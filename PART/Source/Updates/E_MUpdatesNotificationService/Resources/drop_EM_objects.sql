IF EXISTS (SELECT * FROM sysobjects where name='AA70000T_TMP1')
	DROP TABLE AA70000T_TMP1
GO

IF EXISTS (SELECT * FROM sysobjects where name='AA70300T_TMP1')
	DROP TABLE AA70300T_TMP1
GO

IF EXISTS (SELECT * FROM sysobjects where name='AA70000T_TMP2')
	DROP TABLE AA70000T_TMP2
GO

IF EXISTS (SELECT * FROM sysobjects where name='AA70300T_TMP2')
	DROP TABLE AA70300T_TMP2
GO

IF EXISTS (SELECT * FROM sysobjects where name='T_E_MUpdateStaging_Excluded')
	DROP TABLE T_E_MUpdateStaging_Excluded
GO

IF EXISTS (SELECT * FROM sysobjects where name='usp_CPR_Broker_Prepare')
	DROP PROCEDURE usp_CPR_Broker_Prepare
GO

IF EXISTS (SELECT * FROM sysobjects where name='usp_CPR_Broker_Compare')
	DROP PROCEDURE usp_CPR_Broker_Compare
GO