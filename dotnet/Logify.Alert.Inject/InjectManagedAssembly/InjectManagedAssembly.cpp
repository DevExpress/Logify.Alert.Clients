#include "stdafx.h"
#include "metahost.h"
#include "shellapi.h"

HRESULT InjectDotNetAssembly(
	/* [in] */ LPCWSTR pwzAssemblyPath,
	/* [in] */ LPCWSTR pwzTypeName,
	/* [in] */ LPCWSTR pwzMethodName,
	/* [in] */ LPCWSTR pwzArgument
) {
	HRESULT result;
	ICLRMetaHost *metaHost = NULL;
	ICLRRuntimeInfo *runtimeInfo = NULL;
	ICLRRuntimeHost *runtimeHost = NULL;

	// Load .NET
	result = CLRCreateInstance(CLSID_CLRMetaHost, IID_PPV_ARGS(&metaHost));
	// Replace .NET version with the one you want to load (or which is already loaded)
	result = metaHost->GetRuntime(L"v4.0.30319", IID_PPV_ARGS(&runtimeInfo));
	result = runtimeInfo->GetInterface(CLSID_CLRRuntimeHost, IID_PPV_ARGS(&runtimeHost));
	// Start runtime
	result = runtimeHost->Start();

	// Execute managed assembly
	DWORD returnValue;
	result = runtimeHost->ExecuteInDefaultAppDomain(
		pwzAssemblyPath,
		pwzTypeName,
		pwzMethodName,
		pwzArgument,
		&returnValue);

	// free resources
	if (metaHost != NULL)
		metaHost->Release();
	if (runtimeInfo != NULL)
		runtimeInfo->Release();
	if (runtimeHost != NULL)
		runtimeHost->Release();
	return result;
	// Unload .NET
	//result = runtimeHost->Stop();
}

HRESULT InjectManagedAssemblyCore(_In_ LPCWSTR lpCommand) {
	LPWSTR *szArgList;
	int argCount;
	szArgList = CommandLineToArgvW(lpCommand, &argCount);
	if (szArgList == NULL || argCount < 3)
		return E_FAIL;

	LPCWSTR param;
	if (argCount >= 4)
		param = szArgList[3];
	else
		param = L"";

	HRESULT result = InjectDotNetAssembly(
		//L"C:\\Projects\\logify_sandbox\\LogifyInject\\LogifyInject\\LogifyInject\\bin\\Debug\\LogifyInject.dll",
		//L"DevExpress.Logify.Core.Internal.LogifyInit",
		//L"Run",
		//L""
		szArgList[0],
		szArgList[1],
		szArgList[2],
		param
	);
	LocalFree(szArgList);
	return result;
}