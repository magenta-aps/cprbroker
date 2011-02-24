set dbName =%1

del Deploy\%dbName %.sql

"C:\Program Files\Microsoft SQL Server\90\Tools\Publishing\SqlPubWiz" script -d %dbName % -S 10.20.1.20 -U sa -P Dlph10t Deploy\%dbName %.sql

del Deploy\%dbName %.bz2 /F

"C:\Program Files\7-Zip\7z.exe" a -tbzip2 Deploy\%dbName %.bz2 Deploy\%dbName %.sql

