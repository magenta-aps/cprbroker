copy Patch_Wix.wxs c:\Patch\ /y
cd \Patch

REM ============
REM Build the patch
REM ============

candle Patch_Wix.wxs -out Patch_Wix.wixobj

light Patch_Wix.wixobj -out Patch_Wix.wixmsp

rem light Patch_Wix.wixobj -out Patch_Wix.wixout -xo -b NewSource\GK.WebServices\Personmaster\PersonmasterServiceLibrary\Deploy\Installer\
rem light Patch_Wix.wixout -out Patch_Wix.wixmsp

torch -p -xi OldSource\GK.WebServices\Personmaster\PersonmasterServiceLibrary\Deploy\Installer\bin\Debug\en-US\PersonMasterInstaller.wixpdb NewSource\GK.WebServices\Personmaster\PersonmasterServiceLibrary\Deploy\Installer\bin\Debug\en-US\PersonMasterInstaller.wixpdb -out Patch_Wix.wixmst

cd OldSource\GK.WebServices\Personmaster\PersonmasterServiceLibrary\Deploy\Installer
pyro C:\Patch\Patch_Wix.wixmsp -t MyPatch C:\Patch\Patch_Wix.wixmst -out C:\Patch\Patch_Wix.msp
cd \Patch