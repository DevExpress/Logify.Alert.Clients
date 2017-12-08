#include "stdafx.h"

extern "C" {
	__declspec(dllexport) HRESULT InjectManagedAssembly(_In_ LPCWSTR lpCommand) {
		return InjectManagedAssemblyCore(lpCommand);
	}
}
