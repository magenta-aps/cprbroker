set dbName =%1

REM Extract database
copy P:\Deploy\%dbName %.bz2 Deploy\%dbName %.bz2 /y
"c:\Program Files\7-Zip\7z.exe" e Deploy\%dbName %.bz2 -tbzip2 -oDeploy\ -y

REM Drop connections
sqlcmd -S IDBU02 -d master -Q "DECLARE @SQL varchar(max); SET @SQL = ''; SELECT @SQL = @SQL + 'Kill ' + Convert(varchar, SPId) + ';' FROM MASTER.sys.SysProcesses WHERE DBId = DB_ID('%dbName %_Beemen'); exec (@sql)"

REM Restore database
REM sqlcmd -S IDBU02 -d master -Q "RESTORE DATABASE [%dbName %_Beemen] FROM  DISK = N'C:\Users\mag-bb\Desktop\PART\Deploy\%dbName %' WITH  FILE = 1,  MOVE N'%dbName %' TO N'C:\NTVOL1\DB\SQL2K5\MSSQL.1\MSSQL\Data\%dbName %_Beemen.mdf',  MOVE N'%dbName %_log' TO N'C:\NTVOL1\DB\SQL2K5\MSSQL.1\MSSQL\Data\%dbName %_Beemen_log.ldf',  NOUNLOAD,  REPLACE,  STATS = 10"

sqlcmd -S IDBU02 -d %dbName %_Beemen -i Deploy\%dbName %

