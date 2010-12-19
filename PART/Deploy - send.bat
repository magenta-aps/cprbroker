REM Backup database
sqlcmd -U sa -P Dlph10t -S 10.20.1.20 -d PART -Q "BACKUP DATABASE [PART] TO  DISK = N'C:\Program Files\Microsoft SQL Server\MSSQL.1\MSSQL\Backup\PART.bak' WITH NOFORMAT, INIT,  NAME = N'PART-Full Database Backup', SKIP, NOREWIND, NOUNLOAD,  STATS = 10"

md \\5.11.118.93\c$\Users\mag-bb\Desktop\PART\Deploy\

REM Zip and copy DB backup to server
copy "\\10.20.1.20\C$\Program Files\Microsoft SQL Server\MSSQL.1\MSSQL\Backup\PART.bak" Deploy\PART.bak /Y
del Deploy\PARTDB.bz2 /F
"C:\Program Files\7-Zip\7z.exe" a -tbzip2 Deploy\PARTDB.bz2 Deploy\PART.bak
copy Deploy\PARTDB.bz2 \\5.11.118.93\c$\Users\mag-bb\Desktop\PART\Deploy\PARTDB.bz2 /y

REM Copy code files to server
rd /S /Q Deploy\Source
xcopy Source\*.* Deploy\Source\*.* /E /Y /C /Q /Exclude:Deploy\excl.txt
del Deploy\Source.7z /q
"C:\Program Files\7-Zip\7z.exe" a Deploy\Source.7z Deploy\Source\
copy Deploy\Source.7z \\5.11.118.93\c$\Users\mag-bb\Desktop\PART\Deploy\Source.7z /Y 
