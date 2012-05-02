=============
Description
=============
This application loops through a set of CPR numbers, and does various actions for each, depending on the parameters
Each attempt to run the program will create a folder "Runs\yyyy MM dd HH_mm" that contains
- Registrations folder	: Usually contains the PersonRegistration objects of people in XML format. Be sure to delete this folder as soon as you are done, or it will cause a security hole
- Succeeded.txt			: CPR numbers that succeeded
- Failed.txt			: CPR numbers that failed
- Log.txt				: program log, contains progress and exceptions

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

============
RefreshData
============

Needed parameters:
-------------------
/endType, /source, /partUrl, /appToken, /userToken

Example
--------
BatchClient.exe /envType "BatchClient.RefreshData, BatchClient" /source "data1.txt;data2.txt" /partUrl "http://cprbroker/Services/Part.asmx" /appToken "11111111-2222-3333-4444-555555555555" /userToken MyUser 
