#include <Windows.h>
#include "winhttpStubExports.h"
#include "DLLMain.h"

extern void* g_FunctionTable[];

void PopulateFunctionTable()
{
	HMODULE OriginalModule = LoadLibraryExA("winhttp.dll", NULL, LOAD_LIBRARY_SEARCH_SYSTEM32);

	g_FunctionTable[ 0 ] = GetProcAddress( OriginalModule, "WinHttpPacJsWorkerMain" );
	g_FunctionTable[ 1 ] = GetProcAddress( OriginalModule, "WinHttpSetSecureLegacyServersAppCompat" );
	g_FunctionTable[ 2 ] = GetProcAddress( OriginalModule, "DllCanUnloadNow" );
	g_FunctionTable[ 3 ] = GetProcAddress( OriginalModule, "DllGetClassObject" );
	g_FunctionTable[ 4 ] = GetProcAddress( OriginalModule, "Private1" );
	g_FunctionTable[ 5 ] = GetProcAddress( OriginalModule, "SvchostPushServiceGlobals" );
	g_FunctionTable[ 6 ] = GetProcAddress( OriginalModule, "WinHttpAddRequestHeaders" );
	g_FunctionTable[ 7 ] = GetProcAddress( OriginalModule, "WinHttpAddRequestHeadersEx" );
	g_FunctionTable[ 8 ] = GetProcAddress( OriginalModule, "WinHttpAutoProxySvcMain" );
	g_FunctionTable[ 9 ] = GetProcAddress( OriginalModule, "WinHttpCheckPlatform" );
	g_FunctionTable[ 10 ] = GetProcAddress( OriginalModule, "WinHttpCloseHandle" );
	g_FunctionTable[ 11 ] = GetProcAddress( OriginalModule, "WinHttpConnect" );
	g_FunctionTable[ 12 ] = GetProcAddress( OriginalModule, "WinHttpConnectionDeletePolicyEntries" );
	g_FunctionTable[ 13 ] = GetProcAddress( OriginalModule, "WinHttpConnectionDeleteProxyInfo" );
	g_FunctionTable[ 14 ] = GetProcAddress( OriginalModule, "WinHttpConnectionFreeNameList" );
	g_FunctionTable[ 15 ] = GetProcAddress( OriginalModule, "WinHttpConnectionFreeProxyInfo" );
	g_FunctionTable[ 16 ] = GetProcAddress( OriginalModule, "WinHttpConnectionFreeProxyList" );
	g_FunctionTable[ 17 ] = GetProcAddress( OriginalModule, "WinHttpConnectionGetNameList" );
	g_FunctionTable[ 18 ] = GetProcAddress( OriginalModule, "WinHttpConnectionGetProxyInfo" );
	g_FunctionTable[ 19 ] = GetProcAddress( OriginalModule, "WinHttpConnectionGetProxyList" );
	g_FunctionTable[ 20 ] = GetProcAddress( OriginalModule, "WinHttpConnectionSetPolicyEntries" );
	g_FunctionTable[ 21 ] = GetProcAddress( OriginalModule, "WinHttpConnectionSetProxyInfo" );
	g_FunctionTable[ 22 ] = GetProcAddress( OriginalModule, "WinHttpConnectionUpdateIfIndexTable" );
	g_FunctionTable[ 23 ] = GetProcAddress( OriginalModule, "WinHttpCrackUrl" );
	g_FunctionTable[ 24 ] = GetProcAddress( OriginalModule, "WinHttpCreateProxyResolver" );
	g_FunctionTable[ 25 ] = GetProcAddress( OriginalModule, "WinHttpCreateUrl" );
	g_FunctionTable[ 26 ] = GetProcAddress( OriginalModule, "WinHttpDetectAutoProxyConfigUrl" );
	g_FunctionTable[ 27 ] = GetProcAddress( OriginalModule, "WinHttpFreeProxyResult" );
	g_FunctionTable[ 28 ] = GetProcAddress( OriginalModule, "WinHttpFreeProxyResultEx" );
	g_FunctionTable[ 29 ] = GetProcAddress( OriginalModule, "WinHttpFreeProxySettings" );
	g_FunctionTable[ 30 ] = GetProcAddress( OriginalModule, "WinHttpGetDefaultProxyConfiguration" );
	g_FunctionTable[ 31 ] = GetProcAddress( OriginalModule, "WinHttpGetIEProxyConfigForCurrentUser" );
	g_FunctionTable[ 32 ] = GetProcAddress( OriginalModule, "WinHttpGetProxyForUrl" );
	g_FunctionTable[ 33 ] = GetProcAddress( OriginalModule, "WinHttpGetProxyForUrlEx" );
	g_FunctionTable[ 34 ] = GetProcAddress( OriginalModule, "WinHttpGetProxyForUrlEx2" );
	g_FunctionTable[ 35 ] = GetProcAddress( OriginalModule, "WinHttpGetProxyForUrlHvsi" );
	g_FunctionTable[ 36 ] = GetProcAddress( OriginalModule, "WinHttpGetProxyResult" );
	g_FunctionTable[ 37 ] = GetProcAddress( OriginalModule, "WinHttpGetProxyResultEx" );
	g_FunctionTable[ 38 ] = GetProcAddress( OriginalModule, "WinHttpGetProxySettingsVersion" );
	g_FunctionTable[ 39 ] = GetProcAddress( OriginalModule, "WinHttpGetTunnelSocket" );
	g_FunctionTable[ 40 ] = GetProcAddress( OriginalModule, "WinHttpOpen" );
	g_FunctionTable[ 41 ] = GetProcAddress( OriginalModule, "WinHttpOpenRequest" );
	g_FunctionTable[ 42 ] = GetProcAddress( OriginalModule, "WinHttpProbeConnectivity" );
	g_FunctionTable[ 43 ] = GetProcAddress( OriginalModule, "WinHttpQueryAuthSchemes" );
	g_FunctionTable[ 44 ] = GetProcAddress( OriginalModule, "WinHttpQueryDataAvailable" );
	g_FunctionTable[ 45 ] = GetProcAddress( OriginalModule, "WinHttpQueryHeaders" );
	g_FunctionTable[ 46 ] = GetProcAddress( OriginalModule, "WinHttpQueryOption" );
	g_FunctionTable[ 47 ] = GetProcAddress( OriginalModule, "WinHttpReadData" );
	g_FunctionTable[ 48 ] = GetProcAddress( OriginalModule, "WinHttpReadProxySettings" );
	g_FunctionTable[ 49 ] = GetProcAddress( OriginalModule, "WinHttpReadProxySettingsHvsi" );
	g_FunctionTable[ 50 ] = GetProcAddress( OriginalModule, "WinHttpReceiveResponse" );
	g_FunctionTable[ 51 ] = GetProcAddress( OriginalModule, "WinHttpResetAutoProxy" );
	g_FunctionTable[ 52 ] = GetProcAddress( OriginalModule, "WinHttpSaveProxyCredentials" );
	g_FunctionTable[ 53 ] = GetProcAddress( OriginalModule, "WinHttpSendRequest" );
	g_FunctionTable[ 54 ] = GetProcAddress( OriginalModule, "WinHttpSetCredentials" );
	g_FunctionTable[ 55 ] = GetProcAddress( OriginalModule, "WinHttpSetDefaultProxyConfiguration" );
	g_FunctionTable[ 56 ] = GetProcAddress( OriginalModule, "WinHttpSetOption" );
	g_FunctionTable[ 57 ] = GetProcAddress( OriginalModule, "WinHttpSetProxySettingsPerUser" );
	g_FunctionTable[ 58 ] = GetProcAddress( OriginalModule, "WinHttpSetStatusCallback" );
	g_FunctionTable[ 59 ] = GetProcAddress( OriginalModule, "WinHttpSetTimeouts" );
	g_FunctionTable[ 60 ] = GetProcAddress( OriginalModule, "WinHttpTimeFromSystemTime" );
	g_FunctionTable[ 61 ] = GetProcAddress( OriginalModule, "WinHttpTimeToSystemTime" );
	g_FunctionTable[ 62 ] = GetProcAddress( OriginalModule, "WinHttpWebSocketClose" );
	g_FunctionTable[ 63 ] = GetProcAddress( OriginalModule, "WinHttpWebSocketCompleteUpgrade" );
	g_FunctionTable[ 64 ] = GetProcAddress( OriginalModule, "WinHttpWebSocketQueryCloseStatus" );
	g_FunctionTable[ 65 ] = GetProcAddress( OriginalModule, "WinHttpWebSocketReceive" );
	g_FunctionTable[ 66 ] = GetProcAddress( OriginalModule, "WinHttpWebSocketSend" );
	g_FunctionTable[ 67 ] = GetProcAddress( OriginalModule, "WinHttpWebSocketShutdown" );
	g_FunctionTable[ 68 ] = GetProcAddress( OriginalModule, "WinHttpWriteData" );
	g_FunctionTable[ 69 ] = GetProcAddress( OriginalModule, "WinHttpWriteProxySettings" );
}


void CheckModdedLaunchFlag()
{
	LPWSTR lpwCmdLine = GetCommandLineW();
	LPWSTR moddedFlag = L"-modded";
	if (wcsstr(lpwCmdLine, moddedFlag) != NULL) {
		LoadLibraryA("ManagedModFramework\\CLRHost.Dll");
	}
}

DWORD WINAPI ProcessAttach(_In_ LPVOID Parameter)
{
	if ( Parameter == NULL )
		return FALSE;

	PopulateFunctionTable();
	CheckModdedLaunchFlag();

	return TRUE;
}

DWORD WINAPI ProcessDetach(_In_ LPVOID Parameter)
{
	if ( Parameter == NULL )
		return FALSE;

	return TRUE;
}
BOOL APIENTRY DllMain( 
	_In_ HINSTANCE Instance,
	_In_ DWORD     Reason,
	_In_ LPVOID    Reserved 
)
{
	switch ( Reason )
	{
		case DLL_PROCESS_ATTACH:
			DisableThreadLibraryCalls( Instance ); // Disable DLL_THREAD_ATTACH and DLL_THREAD_DETACH notifications
			return ProcessAttach( Instance );
		case DLL_PROCESS_DETACH:
			return ProcessDetach( Instance );
	}

	return TRUE;
}
