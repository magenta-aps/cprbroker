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
/pmUrl		: URL to PersonmasterServiceLibrary.BasicOp.svc
/pmSpn		: SpnName to be used with person master
/maxThreads	: Number of concurrent threads to execute

============
RefreshData
============

Needed parameters:
-------------------
/endType, /source, /partUrl, /appToken, /userToken

========
Example
========

Refresh data
-------------
BatchClient.exe /envType "BatchClient.RefreshData, BatchClient" /source "data1.txt;data2.txt" /partUrl "http://cprbroker/Services/Part.asmx" /appToken "11111111-2222-3333-4444-555555555555" /userToken MyUser 

Get uuids
---------
BatchClient.exe /envType "BatchClient.GetUuids, BatchClient" /source "data1.txt;data2.txt" /partUrl "http://cprbroker/Services/Part.asmx" /appToken "11111111-2222-3333-4444-555555555555" /userToken MyUser 

Regenerate KMD
--------------
BatchClient.exe /envType "BatchClient.RegenerateKMD, BatchClient" /brokerDb "data source=dbserver; initial catalog=cprbroker; integrated security=sspi; user id=; password=;" /appToken "11111111-2222-3333-4444-555555555555" /userToken MyUser

Regenerate CPR Direct
--------------
BatchClient.exe /envType "BatchClient.RegenerateCprDirect, BatchClient" /brokerDb "data source=dbserver; initial catalog=cprbroker; integrated security=sspi; user id=; password=;" /appToken "11111111-2222-3333-4444-555555555555" /userToken MyUser

Regenerate Children
-------------------
BatchClient.exe /envType "BatchClient.RegenerateChildren, BatchClient" /brokerDb "data source=dbserver; initial catalog=cprbroker; integrated security=sspi; user id=; password=;" /appToken "11111111-2222-3333-4444-555555555555" /userToken MyUser

Sorting by effect date
PersonRegistration.Contents column by effect date (attributes and relationships)
----------------------
BatchClient.exe /envType "BatchClient.SortContentsByDate, BatchClient" /brokerDb "data source=dbserver; initial catalog=cprbroker; integrated security=sspi; user id=; password=;"
 


========================
Creating the data file
======================

DPR with foreign nationalities :
--------------------------------
select PNR from DTTOTAL T where 
(
  NOT EXISTS(select * from DTSTAT S where S.PNR=T.PNR and s.ANNKOR is null and HAENSLUT is null)
)
OR
(
  EXISTS (select * from DTSTAT S where S.PNR=T.PNR and s.ANNKOR is null and HAENSLUT is null and S.MYNKOD <> 5100) 
)

