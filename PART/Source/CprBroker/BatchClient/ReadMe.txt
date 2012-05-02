===============
All Parameters
===============
BatchClient.exe param1 param1Value param2 param2Value

/envType	: Type of object to create. Should be inherited from CprBroker.Utilities.ConsoleApps.ConsoleEnvironment
/source		: File name that contains all cpr numbers, each on a single line
/startPnr	: ignore the cpr numbers before this number. This can be used if you want to restart the process from the middle, rather than repeat the whole batch again
/partUrl	: URL to Part.asmx
/brokerDb	: Connection string to CPR broker database
/otherDb	: Connection string to any other database (depends on actual task)
/appToken	: Application token passed to CPR broker
/userToken	: User token passed to CPR broker

Example
----------
DPRClientTester.exe AllCprNumbers.txt

============
RefreshData
============

Needed parameters:
-------------------
/endType, /source, /partUrl, /appToken, /userToken

Example
--------
BatchClient.exe /envType "BatchClient.RefreshData, BatchClient" /source "data1.txt;data2.txt" /partUrl "http://localhost:1551/Services/Part.asmx" /appToken "c6c62a70-88d9-486f-b33c-99f39637d186" /userToken Beemen 
