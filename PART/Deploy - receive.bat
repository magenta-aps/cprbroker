REM Extract database
"c:\Program Files (x86)\7-Zip\7z.exe" e Deploy\PARTDB.7z -oDeploy\ -y

REM Drop connections
sqlcmd -S localhost -d master -Q "DECLARE @SQL varchar(max); SET @SQL = ''; SELECT @SQL = @SQL + 'Kill ' + Convert(varchar, SPId) + ';' FROM MASTER.sys.SysProcesses WHERE DBId = DB_ID('PART_Beemen'); exec (@sql)"

REM Restore database
sqlcmd -S localhost -d master -Q "RESTORE DATABASE [PART_Beemen] FROM  DISK = N'C:\Users\mag-bb\Desktop\PART\Deploy\PART.bak' WITH  FILE = 1,  MOVE N'PART' TO N'C:\NTVOL1\DB\SQL2K5\MSSQL.1\MSSQL\Data\PART_Beemen.mdf',  MOVE N'PART_log' TO N'C:\NTVOL1\DB\SQL2K5\MSSQL.1\MSSQL\Data\PART_Beemen_log.ldf',  NOUNLOAD,  REPLACE,  STATS = 10"


REM extract code files
del /S /Q Deploy\Source\*.*
"c:\Program Files (x86)\7-Zip\7z.exe" x Deploy\Source.7z -y -r
XCOPY Deploy\Source\*.* Source\ /E /Y /C /Q
del /S /Q Deploy\Source\*.*