#include <Windows.h>
#include "versionStubExports.h"
#include "DLLMain.h"

extern void* g_FunctionTable[];

static inline void PopulateFunctionTable() {

	// version.dll is not called in the same working dir as winhttp.dll, so the mod path string must be altered to use it.
	HMODULE OriginalModule = LoadLibraryA("C:\\Windows\\System32\\version.dll");

	g_FunctionTable[0] = GetProcAddress(OriginalModule, "GetFileVersionInfoA");
	g_FunctionTable[1] = GetProcAddress(OriginalModule, "GetFileVersionInfoByHandle");
	g_FunctionTable[2] = GetProcAddress(OriginalModule, "GetFileVersionInfoExA");
	g_FunctionTable[3] = GetProcAddress(OriginalModule, "GetFileVersionInfoExW");
	g_FunctionTable[4] = GetProcAddress(OriginalModule, "GetFileVersionInfoSizeA");
	g_FunctionTable[5] = GetProcAddress(OriginalModule, "GetFileVersionInfoSizeExA");
	g_FunctionTable[6] = GetProcAddress(OriginalModule, "GetFileVersionInfoSizeExW");
	g_FunctionTable[7] = GetProcAddress(OriginalModule, "GetFileVersionInfoSizeW");
	g_FunctionTable[8] = GetProcAddress(OriginalModule, "GetFileVersionInfoW");
	g_FunctionTable[9] = GetProcAddress(OriginalModule, "VerFindFileA");
	g_FunctionTable[10] = GetProcAddress(OriginalModule, "VerFindFileW");
	g_FunctionTable[11] = GetProcAddress(OriginalModule, "VerInstallFileA");
	g_FunctionTable[12] = GetProcAddress(OriginalModule, "VerInstallFileW");
	g_FunctionTable[13] = GetProcAddress(OriginalModule, "VerLanguageNameA");
	g_FunctionTable[14] = GetProcAddress(OriginalModule, "VerLanguageNameW");
	g_FunctionTable[15] = GetProcAddress(OriginalModule, "VerQueryValueA");
	g_FunctionTable[16] = GetProcAddress(OriginalModule, "VerQueryValueW");
}


void CheckModdedLaunchFlag() {

	LPWSTR lpwCmdLine = GetCommandLineW();
	LPWSTR moddedFlag = L"-modded";
	if (wcsstr(lpwCmdLine, moddedFlag) != NULL) {
		LoadLibraryA("RealLoaderFramework\\CLRHost.Dll");
	}
}

DWORD WINAPI ProcessAttach(_In_ LPVOID Parameter) {

	if (Parameter == NULL)
		return FALSE;

	PopulateFunctionTable();
	CheckModdedLaunchFlag();

	return TRUE;
}

DWORD WINAPI ProcessDetach(_In_ LPVOID Parameter) {

	if (Parameter == NULL)
		return FALSE;

	return TRUE;
}

BOOL APIENTRY DllMain(
	_In_ HINSTANCE Instance,
	_In_ DWORD     Reason,
	_In_ LPVOID    Reserved
)
{
	switch (Reason)
	{
		case DLL_PROCESS_ATTACH:
			DisableThreadLibraryCalls(Instance); // Disable DLL_THREAD_ATTACH and DLL_THREAD_DETACH notifications
			return ProcessAttach(Instance);
		case DLL_PROCESS_DETACH:
			return ProcessDetach(Instance);
	}

	return TRUE;
}
