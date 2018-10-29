@echo off
rem nuget pack Logify.Alert.Core.csproj -Verbosity detailed -Prop Platform=AnyCPU -Prop Configuration=Release40
rem nuget pack Logify.Alert.Core.csproj -Verbosity detailed -Prop Platform=AnyCPU -Prop Configuration=Release45
rem nuget pack Logify.Alert.Core.NetCore.csproj -Verbosity detailed -Prop Platform=AnyCPU -Prop Configuration=Release
dotnet restore Logify.Alert.NetCore.Core.csproj
dotnet build Logify.Alert.NetCore.Core.csproj
dotnet pack Logify.Alert.NetCore.Core.csproj