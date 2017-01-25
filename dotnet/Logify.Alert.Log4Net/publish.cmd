@echo off
nuget pack Logify.Alert.Log4Net.csproj -Verbosity detailed -Prop Platform=AnyCPU -Prop Configuration=Release40
nuget pack Logify.Alert.Log4Net.csproj -Verbosity detailed -Prop Platform=AnyCPU -Prop Configuration=Release45