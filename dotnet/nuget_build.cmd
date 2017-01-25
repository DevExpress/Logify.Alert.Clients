@echo on

rd tmp /Q /S
mkdir tmp

pushd tmp
mkdir latestclients
mkdir latestclients\net40
mkdir latestclients\net45

set artifactsDir=%1
set clientPath=%1

popd

copy %clientPath%\net40\*.* .\latestclients\net40
copy %clientPath%\net45\*.* .\latestclients\net45

call :buildclient Logify.Alert.Core
call :buildclient Logify.Alert.Win
call :buildclient Logify.Alert.Web
call :buildclient Logify.Alert.Wpf
call :buildclient Logify.Alert.Log4Net
call :buildclient Logify.Alert.NLog
call :buildclient Logify.Alert.Serilog
rd tmp /Q /S

goto finish

:buildclient
pushd %1
nuget restore
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
for %%i in (.\unpacked\lib\net40\*.dll) do copy .\latestclients\net40\%%~nxi .\unpacked\lib\net40\ /Y
for %%i in (.\unpacked\lib\net45\*.dll) do copy .\latestclients\net45\%%~nxi .\unpacked\lib\net45\ /Y
pushd unpacked
pkzipc -add -recurse -path temp.zip *.*
move temp.zip ..\..
popd
rd unpacked /Q /S
popd
del %nupkgName%
rename temp.zip %nupkgName%
exit /b

:finish