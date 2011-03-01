REM extract code files
del /S /Q Deploy\Source\*.*
copy P:\Deploy\Source.7z Deploy\Source.7z /y
"c:\Program Files\7-Zip\7z.exe" x Deploy\Source.7z -y -r
XCOPY Deploy\Source\*.* Source\ /E /Y /C /Q
del /S /Q Deploy\Source\*.*