@echo off
nuget pack Logify.Alert.Core.csproj -Verbosity detailed -Prop Platform=AnyCPU -Prop Configuration=Release40
nuget pack Logify.Alert.Core.csproj -Verbosity detailed -Prop Platform=AnyCPU -Prop Configuration=Release45