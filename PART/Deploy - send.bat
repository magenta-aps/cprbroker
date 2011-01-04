REM Backup databases
CALL SendDatabase Part
CALL SendDatabase PartEventBroker


REM md \\5.32.134.68\c$\Users\mag-bb\Desktop\PART\Deploy\

REM Copy code files to server
rd /S /Q Deploy\Source
xcopy Source\*.* Deploy\Source\*.* /E /Y /C /Q /Exclude:Deploy\excl.txt
del Deploy\Source.7z /q
"C:\Program Files\7-Zip\7z.exe" a Deploy\Source.7z Deploy\Source\
REM copy Deploy\Source.7z \\5.32.134.68\c$\Users\mag-bb\Desktop\PART\Deploy\Source.7z /Y 
