@echo off

call "%VS140COMNTOOLS%\VsDevCmd.bat"
set MSBUILD4=msbuild

set CfgDebug=Debug
set ThreadsCount=1
set nuget=%CD%\nuget\NuGet.exe

set p1=%1%

if "%p1%"=="" call :all

:all
echo ==== Building ====
call :build %CfgDebug% "%CD%\Logify.Alert.DotNet.sln"

exit /b

:build
echo ==== Building %3 %time% =====
%nuget% restore "%CD%\Logify.Alert.DotNet.sln"
%MSBUILD4% %4 /nologo /m:%ThreadsCount% /t:rebuild /clp:errorsonly /property:AllowUnsafeBlocks=true;Configuration=%1 %2 >> "%CD%\build.log"
echo ==== Done %3 %time% =====
if %errorlevel%==0 exit /b
echo --- %3 has ERRORS ---
set HASERRORS=1
exit -1 /b