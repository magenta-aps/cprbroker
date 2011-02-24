rd /S /Q Deploy\Source
xcopy Source\*.* Deploy\Source\*.* /E /Y /C /Q /Exclude:Deploy\excl.txt
del Deploy\Source.7z /q
"C:\Program Files\7-Zip\7z.exe" a Deploy\Source.7z Deploy\Source\
