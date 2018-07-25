@echo on

rd tmp /Q /S
mkdir tmp

mkdir tmp\latestclients
mkdir tmp\latestclients\net40
mkdir tmp\latestclients\net45

rem set artifactsDir=%1
set clientPath=%1

copy %clientPath%\net40\*.* .\tmp\latestclients\net40
copy %clientPath%\net45\*.* .\tmp\latestclients\net45

nuget restore

call :buildclient Logify.Alert.Core

call :buildclient Logify.Alert.Console

call :buildclient Logify.Alert.Win

call :buildclient Logify.Alert.Web

call :buildclient Logify.Alert.Wpf

call :buildclient Logify.Alert.Log4Net

call :buildclient Logify.Alert.NLog


call :buildclient Logify.Alert.Serilog

call :buildclient Logify.Alert.Xamarin.Android




rd tmp /Q /S



call :mergenupkgbymask . ..\bin\ Logify.Alert.Core.*.nupkg .




call :mergenupkgbymask . ..\bin\ Logify.Alert.Console.*.nupkg .




call :mergenupkgbymask . ..\bin\ Logify.Alert.Web.*.nupkg .





rem for %%i in (..\bin\Logify.Alert.Console.*.nupkg) do set nupkgName=%%i
rem for %%i in (.\Logify.Alert.Win.*.nupkg) do set winnupkgname=%%i
rem SET targetnupkgname=%winnupkgname:Logify.Alert.Win=Logify.Alert.Console%
rem call :patchnetcorepackage %nupkgName% %targetnupkgname%

goto finish

:buildclient
pushd %1
call publish.cmd
move *.nupkg ..\tmp
popd

pushd tmp
mkdir unpacked
set nupkgName = "";
for %%i in (*.nupkg) do set nupkgName=%%i
for %%i in (*.nupkg) do rename %%i %%i.zip
for %%i in (*.zip) do pkzipc -extract -dir %%i .\unpacked
del *.zip /Q
rem copy signed libs from latestclients directory over existing in package
for %%i in (.\unpacked\lib\net40\*.dll) do copy .\latestclients\net40\%%~nxi .\unpacked\lib\net40\ /Y
for %%i in (.\unpacked\lib\net45\*.dll) do copy .\latestclients\net45\%%~nxi .\unpacked\lib\net45\ /Y
pushd unpacked

rem make package with signed libraries
pkzipc -add -recurse -path temp.zip *.*
move temp.zip ..\..
popd
rd unpacked /Q /S
popd
del %nupkgName%
rename temp.zip %nupkgName%

exit /b









:mergenupkgbymask
for %%i in (%1\%3) do set first=%%i
for %%i in (%2\%3) do set second=%%i
for %%i in (%1\%3) do set target=%4\%%~nxi

call :merge_nupkg %first% %second% %target%
exit /b











:merge_nupkg
call :unpackpackage %1 .\first
call :unpackpackage %2 .\second

for %%i in (.\first\*.nuspec) do set firstnuspec=%%i
for %%i in (.\second\*.nuspec) do set secondnuspec=%%i
mergenuspec %firstnuspec% %secondnuspec% %firstnuspec%
xcopy .\second\lib .\first\lib  /S /Q /Y

rem for %%i in (%1) do set targetFileName=%%~nxi
set targetFileName=%3
call :makepackage .\first %targetFileName%
rd .\first /Q /S
rd .\second /Q /S
exit /b

:unpackpackage
mkdir %2
move %1 %1.zip
pkzipc -extract -dir -over=all %1.zip %2
move %1.zip %1
exit /b

:makepackage
pushd %1
pkzipc -add -recurse -path temp.zip *.*
popd
move %1\temp.zip %2
exit /b

:patchnetcorepackage
set targetFileName=%2
call :unpackpackage %1 .\first
for %%i in (.\first\*.nuspec) do set nuspecname=%%i
for %%i in (.\first\lib\netstandard2.0\*.dll) do set asmname=%%i
for %%i in (.\first\lib\netstandard1.6\*.dll) do set asmname=%%i
for %%i in (.\first\lib\netstandard1.5\*.dll) do set asmname=%%i
for %%i in (.\first\lib\netstandard1.4\*.dll) do set asmname=%%i
dotnet PatchNuspecByAssemblyAttributes.dll %nuspecname% %asmname%
call :makepackage .\first %targetFileName%
rd .\first /Q /S
exit /b











:finish




