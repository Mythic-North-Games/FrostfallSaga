@echo off

for /f "usebackq delims=" %%i in (`vswhere -latest -prerelease -products * -property enginePath`) do (
  set enginePath=%%i
)

if exist "%enginePath%\VSIXInstaller.exe" (
  :: uninstall lite version
  call "%enginePath%\VSIXInstaller.exe" /u:VSIXLite2.6815b720-6186-48a1-a405-1387e54b41c6
  
  :: install full version
  call "%enginePath%\VSIXInstaller.exe" "../Installers~/UnityCodeAssist.Full.VisualStudio.Installer.vsix"
)