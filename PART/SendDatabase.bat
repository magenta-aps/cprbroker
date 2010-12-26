set dbName =%1

sqlcmd -U sa -P Dlph10t -S 10.20.1.20 -d %dbName % -Q "BACKUP DATABASE [%dbName %] TO  DISK = N'C:\Program Files\Microsoft SQL Server\MSSQL.1\MSSQL\Backup\%dbName %.bak' WITH NOFORMAT, INIT,  NAME = N'%dbName %-Full Database Backup', SKIP, NOREWIND, NOUNLOAD,  STATS = 10"

copy "\\10.20.1.20\C$\Program Files\Microsoft SQL Server\MSSQL.1\MSSQL\Backup\%dbName %.bak" Deploy\%dbName %.bak /Y

del Deploy\%dbName %.bz2 /F
"C:\Program Files\7-Zip\7z.exe" a -tbzip2 Deploy\%dbName %.bz2 Deploy\%dbName %.bak

copy Deploy\%dbName %.bz2 \\5.11.118.93\c$\Users\mag-bb\Desktop\PART\Deploy\%dbName %.bz2 /y