set dbName =%1

REM Extract database
copy P:\Deploy\%dbName %.bz2 Deploy\%dbName %.bz2 /y
"c:\Program Files\7-Zip\7z.exe" e Deploy\%dbName %.bz2 -tbzip2 -oDeploy\ -y

REM Drop connections
sqlcmd -S IDBU02 -d master -Q "DECLARE @SQL varchar(max); SET @SQL = ''; SELECT @SQL = @SQL + 'Kill ' + Convert(varchar, SPId) + ';' FROM MASTER.sys.SysProcesses WHERE DBId = DB_ID('%dbName %_Beemen'); exec (@sql)"

REM Restore database

sqlcmd -S IDBU02 -d %dbName %_Beemen -i Deploy\%dbName %

