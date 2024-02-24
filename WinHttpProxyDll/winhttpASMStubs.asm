.DATA
g_FunctionTable QWORD 70 dup(?)
PUBLIC g_FunctionTable

.CODE
WinHttpPacJsWorkerMain PROC
	jmp [g_FunctionTable + 0 * 8]
WinHttpPacJsWorkerMain ENDP

WinHttpSetSecureLegacyServersAppCompat PROC
	jmp [g_FunctionTable + 1 * 8]
WinHttpSetSecureLegacyServersAppCompat ENDP

DllCanUnloadNow PROC
	jmp [g_FunctionTable + 2 * 8]
DllCanUnloadNow ENDP

DllGetClassObject PROC
	jmp [g_FunctionTable + 3 * 8]
DllGetClassObject ENDP

Private1 PROC
	jmp [g_FunctionTable + 4 * 8]
Private1 ENDP

SvchostPushServiceGlobals PROC
	jmp [g_FunctionTable + 5 * 8]
SvchostPushServiceGlobals ENDP

WinHttpAddRequestHeaders PROC
	jmp [g_FunctionTable + 6 * 8]
WinHttpAddRequestHeaders ENDP

WinHttpAddRequestHeadersEx PROC
	jmp [g_FunctionTable + 7 * 8]
WinHttpAddRequestHeadersEx ENDP

WinHttpAutoProxySvcMain PROC
	jmp [g_FunctionTable + 8 * 8]
WinHttpAutoProxySvcMain ENDP

WinHttpCheckPlatform PROC
	jmp [g_FunctionTable + 9 * 8]
WinHttpCheckPlatform ENDP

WinHttpCloseHandle PROC
	jmp [g_FunctionTable + 10 * 8]
WinHttpCloseHandle ENDP

WinHttpConnect PROC
	jmp [g_FunctionTable + 11 * 8]
WinHttpConnect ENDP

WinHttpConnectionDeletePolicyEntries PROC
	jmp [g_FunctionTable + 12 * 8]
WinHttpConnectionDeletePolicyEntries ENDP

WinHttpConnectionDeleteProxyInfo PROC
	jmp [g_FunctionTable + 13 * 8]
WinHttpConnectionDeleteProxyInfo ENDP

WinHttpConnectionFreeNameList PROC
	jmp [g_FunctionTable + 14 * 8]
WinHttpConnectionFreeNameList ENDP

WinHttpConnectionFreeProxyInfo PROC
	jmp [g_FunctionTable + 15 * 8]
WinHttpConnectionFreeProxyInfo ENDP

WinHttpConnectionFreeProxyList PROC
	jmp [g_FunctionTable + 16 * 8]
WinHttpConnectionFreeProxyList ENDP

WinHttpConnectionGetNameList PROC
	jmp [g_FunctionTable + 17 * 8]
WinHttpConnectionGetNameList ENDP

WinHttpConnectionGetProxyInfo PROC
	jmp [g_FunctionTable + 18 * 8]
WinHttpConnectionGetProxyInfo ENDP

WinHttpConnectionGetProxyList PROC
	jmp [g_FunctionTable + 19 * 8]
WinHttpConnectionGetProxyList ENDP

WinHttpConnectionSetPolicyEntries PROC
	jmp [g_FunctionTable + 20 * 8]
WinHttpConnectionSetPolicyEntries ENDP

WinHttpConnectionSetProxyInfo PROC
	jmp [g_FunctionTable + 21 * 8]
WinHttpConnectionSetProxyInfo ENDP

WinHttpConnectionUpdateIfIndexTable PROC
	jmp [g_FunctionTable + 22 * 8]
WinHttpConnectionUpdateIfIndexTable ENDP

WinHttpCrackUrl PROC
	jmp [g_FunctionTable + 23 * 8]
WinHttpCrackUrl ENDP

WinHttpCreateProxyResolver PROC
	jmp [g_FunctionTable + 24 * 8]
WinHttpCreateProxyResolver ENDP

WinHttpCreateUrl PROC
	jmp [g_FunctionTable + 25 * 8]
WinHttpCreateUrl ENDP

WinHttpDetectAutoProxyConfigUrl PROC
	jmp [g_FunctionTable + 26 * 8]
WinHttpDetectAutoProxyConfigUrl ENDP

WinHttpFreeProxyResult PROC
	jmp [g_FunctionTable + 27 * 8]
WinHttpFreeProxyResult ENDP

WinHttpFreeProxyResultEx PROC
	jmp [g_FunctionTable + 28 * 8]
WinHttpFreeProxyResultEx ENDP

WinHttpFreeProxySettings PROC
	jmp [g_FunctionTable + 29 * 8]
WinHttpFreeProxySettings ENDP

WinHttpGetDefaultProxyConfiguration PROC
	jmp [g_FunctionTable + 30 * 8]
WinHttpGetDefaultProxyConfiguration ENDP

WinHttpGetIEProxyConfigForCurrentUser PROC
	jmp [g_FunctionTable + 31 * 8]
WinHttpGetIEProxyConfigForCurrentUser ENDP

WinHttpGetProxyForUrl PROC
	jmp [g_FunctionTable + 32 * 8]
WinHttpGetProxyForUrl ENDP

WinHttpGetProxyForUrlEx PROC
	jmp [g_FunctionTable + 33 * 8]
WinHttpGetProxyForUrlEx ENDP

WinHttpGetProxyForUrlEx2 PROC
	jmp [g_FunctionTable + 34 * 8]
WinHttpGetProxyForUrlEx2 ENDP

WinHttpGetProxyForUrlHvsi PROC
	jmp [g_FunctionTable + 35 * 8]
WinHttpGetProxyForUrlHvsi ENDP

WinHttpGetProxyResult PROC
	jmp [g_FunctionTable + 36 * 8]
WinHttpGetProxyResult ENDP

WinHttpGetProxyResultEx PROC
	jmp [g_FunctionTable + 37 * 8]
WinHttpGetProxyResultEx ENDP

WinHttpGetProxySettingsVersion PROC
	jmp [g_FunctionTable + 38 * 8]
WinHttpGetProxySettingsVersion ENDP

WinHttpGetTunnelSocket PROC
	jmp [g_FunctionTable + 39 * 8]
WinHttpGetTunnelSocket ENDP

WinHttpOpen PROC
	jmp [g_FunctionTable + 40 * 8]
WinHttpOpen ENDP

WinHttpOpenRequest PROC
	jmp [g_FunctionTable + 41 * 8]
WinHttpOpenRequest ENDP

WinHttpProbeConnectivity PROC
	jmp [g_FunctionTable + 42 * 8]
WinHttpProbeConnectivity ENDP

WinHttpQueryAuthSchemes PROC
	jmp [g_FunctionTable + 43 * 8]
WinHttpQueryAuthSchemes ENDP

WinHttpQueryDataAvailable PROC
	jmp [g_FunctionTable + 44 * 8]
WinHttpQueryDataAvailable ENDP

WinHttpQueryHeaders PROC
	jmp [g_FunctionTable + 45 * 8]
WinHttpQueryHeaders ENDP

WinHttpQueryOption PROC
	jmp [g_FunctionTable + 46 * 8]
WinHttpQueryOption ENDP

WinHttpReadData PROC
	jmp [g_FunctionTable + 47 * 8]
WinHttpReadData ENDP

WinHttpReadProxySettings PROC
	jmp [g_FunctionTable + 48 * 8]
WinHttpReadProxySettings ENDP

WinHttpReadProxySettingsHvsi PROC
	jmp [g_FunctionTable + 49 * 8]
WinHttpReadProxySettingsHvsi ENDP

WinHttpReceiveResponse PROC
	jmp [g_FunctionTable + 50 * 8]
WinHttpReceiveResponse ENDP

WinHttpResetAutoProxy PROC
	jmp [g_FunctionTable + 51 * 8]
WinHttpResetAutoProxy ENDP

WinHttpSaveProxyCredentials PROC
	jmp [g_FunctionTable + 52 * 8]
WinHttpSaveProxyCredentials ENDP

WinHttpSendRequest PROC
	jmp [g_FunctionTable + 53 * 8]
WinHttpSendRequest ENDP

WinHttpSetCredentials PROC
	jmp [g_FunctionTable + 54 * 8]
WinHttpSetCredentials ENDP

WinHttpSetDefaultProxyConfiguration PROC
	jmp [g_FunctionTable + 55 * 8]
WinHttpSetDefaultProxyConfiguration ENDP

WinHttpSetOption PROC
	jmp [g_FunctionTable + 56 * 8]
WinHttpSetOption ENDP

WinHttpSetProxySettingsPerUser PROC
	jmp [g_FunctionTable + 57 * 8]
WinHttpSetProxySettingsPerUser ENDP

WinHttpSetStatusCallback PROC
	jmp [g_FunctionTable + 58 * 8]
WinHttpSetStatusCallback ENDP

WinHttpSetTimeouts PROC
	jmp [g_FunctionTable + 59 * 8]
WinHttpSetTimeouts ENDP

WinHttpTimeFromSystemTime PROC
	jmp [g_FunctionTable + 60 * 8]
WinHttpTimeFromSystemTime ENDP

WinHttpTimeToSystemTime PROC
	jmp [g_FunctionTable + 61 * 8]
WinHttpTimeToSystemTime ENDP

WinHttpWebSocketClose PROC
	jmp [g_FunctionTable + 62 * 8]
WinHttpWebSocketClose ENDP

WinHttpWebSocketCompleteUpgrade PROC
	jmp [g_FunctionTable + 63 * 8]
WinHttpWebSocketCompleteUpgrade ENDP

WinHttpWebSocketQueryCloseStatus PROC
	jmp [g_FunctionTable + 64 * 8]
WinHttpWebSocketQueryCloseStatus ENDP

WinHttpWebSocketReceive PROC
	jmp [g_FunctionTable + 65 * 8]
WinHttpWebSocketReceive ENDP

WinHttpWebSocketSend PROC
	jmp [g_FunctionTable + 66 * 8]
WinHttpWebSocketSend ENDP

WinHttpWebSocketShutdown PROC
	jmp [g_FunctionTable + 67 * 8]
WinHttpWebSocketShutdown ENDP

WinHttpWriteData PROC
	jmp [g_FunctionTable + 68 * 8]
WinHttpWriteData ENDP

WinHttpWriteProxySettings PROC
	jmp [g_FunctionTable + 69 * 8]
WinHttpWriteProxySettings ENDP

END
