echo off
echo ===========Dotfuscator==========
cd /d %~dp0 
@rem set currentDir=%~dp0
set isPause=true
if "%1"=="/q" (
set isPause=false
)



msbuild AdvInst.msbuild /t:advinst /p:InstVer=1.2.3.4


echo ===========Clean Ended==========
if %isPause%==true (
echo ===========Clean Paused==========
pause
)
