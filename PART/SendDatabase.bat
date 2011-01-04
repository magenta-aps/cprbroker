set dbName =%1

REM sqlcmd -U sa -P Dlph10t -S 10.20.1.20 -d %dbName % -Q "BACKUP DATABASE [%dbName %] TO  DISK = N'C:\Program Files\Microsoft SQL Server\MSSQL.1\MSSQL\Backup\%dbName %.bak' WITH NOFORMAT, INIT,  NAME = N'%dbName %-Full Database Backup', SKIP, NOREWIND, NOUNLOAD,  STATS = 10"

REM copy "\\10.20.1.20\C$\Program Files\Microsoft SQL Server\MSSQL.1\MSSQL\Backup\%dbName %.bak" Deploy\%dbName %.bak /Y

del Deploy\%dbName %.sql

"C:\Program Files\Microsoft SQL Server\90\Tools\Publishing\SqlPubWiz" script -d %dbName % -S 10.20.1.20 -U sa -P Dlph10t Deploy\%dbName %.sql

del Deploy\%dbName %.bz2 /F

"C:\Program Files\7-Zip\7z.exe" a -tbzip2 Deploy\%dbName %.bz2 Deploy\%dbName %.sql

REM copy Deploy\%dbName %.bz2 \\5.32.134.68\c$\Users\mag-bb\Desktop\PART\Deploy\%dbName %.bz2 /y