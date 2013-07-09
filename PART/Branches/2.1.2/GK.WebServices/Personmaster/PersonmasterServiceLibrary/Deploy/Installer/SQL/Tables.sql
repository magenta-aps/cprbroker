/*==============================================================*/
/* Table: T_CORE_Config                                         */
/*==============================================================*/
EXEC AddTable T_CORE_Config, name,'varchar(60)'
EXEC  AddColumn T_CORE_Config, value,'varchar(1020)'

EXEC AddPrimaryKey T_CORE_Config, name

--EXEC AddSql 'INSERT INTO T_CORE_Config VALUES (''namespace-self'', ''<pm-namespace>'')'
EXEC AddSql 'INSERT INTO T_CORE_Config VALUES (''db-version'', ''1.2'')'
EXEC AddSql 'INSERT INTO T_CORE_Config VALUES (''db-environment'', ''PROD'')'
EXEC AddSql 'INSERT INTO T_CORE_Config VALUES (''loglevel'', ''4''))'

/*==============================================================*/
/* Table: T_CORE_Log                                            */
/*==============================================================*/
EXEC AddTable T_CORE_Log, ID, 'INT IDENTITY'
EXEC AddColumn T_CORE_Log, logTime, 'datetime'
EXEC AddColumn T_CORE_Log, logType, 'int', 1
EXEC AddColumn T_CORE_Log, package, 'int', 1
EXEC AddColumn T_CORE_Log, context, 'varchar(1020)', 1
EXEC AddColumn T_CORE_Log, corrID, 'varchar(60)', 1
EXEC AddColumn T_CORE_Log, spName, 'varchar(120)', 1
EXEC AddColumn T_CORE_Log, eventID, 'integer', 1
EXEC AddColumn T_CORE_Log, logText, 'varchar(2000)', 1

EXEC AddPrimaryKey T_CORE_Log, ID

EXEC AddIndex T_CORE_Log, logTime
EXEC AddIndex T_CORE_Log, logType
EXEC AddIndex T_CORE_Log, package
EXEC AddIndex T_CORE_Log, corrID

/*==============================================================*/
/* Table: T_CORE_LogType                                        */
/*==============================================================*/
EXEC AddTable T_CORE_LogType, typeID, 'int'
EXEC AddColumn T_CORE_LogType, typeName, 'varchar(15)', 1

EXEC AddPrimaryKey T_CORE_LogType, typeID

EXEC AddSql 'INSERT INTO T_CORE_LogType VALUES (-1, ''UNDEFINED'')'
EXEC AddSql 'INSERT INTO T_CORE_LogType VALUES (0, ''RESERVED'')'
EXEC AddSql 'INSERT INTO T_CORE_LogType VALUES (1, ''AUDIT'')'
EXEC AddSql 'INSERT INTO T_CORE_LogType VALUES (2, ''ERROR'')'
EXEC AddSql 'INSERT INTO T_CORE_LogType VALUES (3, ''INFO'')'
EXEC AddSql 'INSERT INTO T_CORE_LogType VALUES (4, ''TRACE'')'
EXEC AddSql 'INSERT INTO T_CORE_LogType VALUES (5, ''DEBUG'')'

/*==============================================================*/
/* Table: T_CORE_Package                                        */
/*==============================================================*/
EXEC AddTable T_CORE_Package, packageID, 'int'
EXEC AddColumn T_CORE_Package, prefix, 'varchar(15)', 1
EXEC AddColumn T_CORE_Package, description, 'varchar(250)', 1

EXEC AddPrimaryKey T_CORE_Package, packageID

EXEC AddSql 'INSERT INTO T_CORE_Package VALUES (-1, ''UNDEF'', ''Undefined'')'
EXEC AddSql 'INSERT INTO T_CORE_Package VALUES (1, ''CORE'', ''Prefix for GK CORE DB and GkApp framework related DB objects'')'
EXEC AddSql 'INSERT INTO T_CORE_Package VALUES (2, ''PM'', ''Prefix for PersonmasterService DB objects'')'
EXEC AddSql 'INSERT INTO T_CORE_Package VALUES (3, ''PMU'', ''Prefix for login/user account handling DB objects'')'
EXEC AddSql 'INSERT INTO T_CORE_Package VALUES (4, ''N2L'', ''Prefix for NemID2LAN DB objects'')'

/*==============================================================*/
/* Table: T_N2L_AccessOverride                                  */
/*==============================================================*/
EXEC AddTable T_N2L_AccessOverride, personMasterID, 'uniqueidentifier'
EXEC AddColumn T_N2L_AccessOverride, createTS, 'datetime', 1
EXEC AddColumn T_N2L_AccessOverride, accessAllowed, 'int', 1

EXEC AddPrimaryKey T_N2L_AccessOverride, personMasterID

/*==============================================================*/
/* Table: T_N2L_NonAdminUser                                    */
/*==============================================================*/
EXEC AddTable T_N2L_NonAdminUser, personMasterID, 'uniqueidentifier'
EXEC AddColumn T_N2L_NonAdminUser, createTS, 'datetime', 1

EXEC AddPrimaryKey T_N2L_NonAdminUser, personMasterID
   
/*==============================================================*/
/* Table: T_PMU_UserAccount                                     */
/*==============================================================*/
EXEC AddTable T_PMU_UserAccount, objectID, 'uniqueidentifier'
EXEC AddColumn T_PMU_UserAccount, loginName, 'varchar(30)', 1
EXEC AddColumn T_PMU_UserAccount, personMasterID, 'uniqueidentifier', 1
EXEC AddColumn T_PMU_UserAccount, transactFrom, 'datetime', 1
EXEC AddColumn T_PMU_UserAccount, transactTo, 'datetime', 1

EXEC AddPrimaryKey T_PMU_UserAccount, objectID, transactTo

EXEC AddIndex T_PMU_UserAccount, personMasterID
EXEC AddIndex T_PMU_UserAccount, loginName, transactTo, 1

/*==============================================================*/
/* Table: T_PMU_UserAccount_Preferred                           */
/*==============================================================*/
EXEC AddTable T_PMU_UserAccount_Preferred, personMasterID, 'uniqueidentifier'
EXEC AddColumn T_PMU_UserAccount_Preferred, uaObjectID,'uniqueidentifier', 1

EXEC AddPrimaryKey T_PMU_UserAccount_Preferred, personMasterID

EXEC AddIndex T_PMU_UserAccount_Preferred, uaObjectID 
EXEC AddIndex T_PMU_UserAccount_Preferred, personMasterID 

/*==============================================================*/
/* Table: T_PM_CPR                                              */
/*==============================================================*/
EXEC AddTable T_PM_CPR, encryptedCprNo, 'varbinary(90)'
EXEC AddColumn T_PM_CPR, birthDate, 'datetime', 1
EXEC AddColumn T_PM_CPR, gender, 'int', 1
EXEC AddColumn T_PM_CPR, personMasterID, 'uniqueidentifier', 1
EXEC AddColumn T_PM_CPR, createTS, 'datetime', 1

EXEC AddPrimaryKey T_PM_CPR, encryptedCprNo

EXEC AddIndex T_PM_CPR, birthDate 
EXEC AddIndex T_PM_CPR, personMasterID 

/*==============================================================*/
/* Table: T_PM_Gender                                           */
/*==============================================================*/
EXEC AddTable T_PM_Gender, ID, 'int'
EXEC AddColumn T_PM_Gender, description, 'varchar(250)', 1

EXEC AddPrimaryKey T_PM_Gender, ID

EXEC AddSql 'INSERT INTO T_PM_Gender VALUES (-1,''Unknown'')'
EXEC AddSql 'INSERT INTO T_PM_Gender VALUES (0,''Female'')'
EXEC AddSql 'INSERT INTO T_PM_Gender VALUES (1,''Male'')'

/*==============================================================*/
/* Table: T_PM_ObjectOwner                                      */
/*==============================================================*/
EXEC AddTable T_PM_ObjectOwner, objectOwnerID, 'uniqueidentifier'
EXEC AddColumn T_PM_ObjectOwner, ownerNamespace, 'varchar(510)', 1
EXEC AddColumn T_PM_ObjectOwner, createTS, 'datetime', 1

EXEC AddPrimaryKey T_PM_ObjectOwner, objectOwnerID

EXEC AddIndex T_PM_ObjectOwner, ownerNamespace

/*==============================================================*/
/* Table: T_PM_PersonMaster                                     */
/*==============================================================*/
EXEC AddTable T_PM_PersonMaster, objectID, 'uniqueidentifier'
EXEC AddColumn T_PM_PersonMaster, objectOwnerID, 'uniqueidentifier', 1
EXEC AddColumn T_PM_PersonMaster, createTS, 'datetime', 1

EXEC AddPrimaryKey T_PM_PersonMaster, objectID

EXEC AddIndex T_PM_PersonMaster, objectOwnerID

/*==============================================================*/
/* Foreign keys                                     */
/*==============================================================*/

EXEC AddForeignKey T_CORE_Log, logType, T_CORE_LogType, typeID

EXEC AddForeignKey T_CORE_Log, package, T_CORE_Package, packageID

EXEC AddForeignKey T_N2L_AccessOverride, personMasterID, T_PM_PersonMaster, objectID

EXEC AddForeignKey T_N2L_NonAdminUser, personMasterID, T_PM_PersonMaster, objectID

EXEC AddForeignKey T_PMU_UserAccount,personMasterID, T_PM_PersonMaster, objectID

EXEC AddForeignKey T_PMU_UserAccount_Preferred, personMasterID, T_PM_PersonMaster, objectID

EXEC AddForeignKey T_PM_CPR, gender, T_PM_Gender, ID

EXEC AddForeignKey T_PM_CPR, personMasterID, T_PM_PersonMaster, objectID

EXEC AddForeignKey T_PM_PersonMaster, objectOwnerID, T_PM_ObjectOwner, objectOwnerID

