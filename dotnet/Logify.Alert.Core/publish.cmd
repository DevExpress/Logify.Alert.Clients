@echo off
nuget pack Logify.Alert.Core.csproj -Verbosity detailed -Prop Platform=AnyCPU -Prop Configuration=Release40
nuget pack Logify.Alert.Core.csproj -Verbosity detailed -Prop Platform=AnyCPU -Prop Configuration=Release45
rem nuget pack Logify.Alert.Core.NetCore.csproj -Verbosity detailed -Prop Platform=AnyCPU -Prop Configuration=Release