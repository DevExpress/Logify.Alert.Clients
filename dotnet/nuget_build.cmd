@echo on

if "%1"=="" (
  ECHO Usage buget_build.cmd <artifacts dir (usually bin)>
  goto finish
)

rd tmp /Q /S
mkdir tmp

pushd tmp
mkdir latestclients

set artifactsDir=%1
set clientPath=%1

popd

copy %clientPath%\*.* .\latestclients

call :buildclient Logify.Alert.Win
call :buildclient Logify.Alert.Web
call :buildclient Logify.Alert.Wpf
rd tmp /Q /S

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
for %%i in (.\unpacked\lib\net40\*.dll) do copy .\latestclients\%%~nxi .\unpacked\lib\net40\ /Y
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