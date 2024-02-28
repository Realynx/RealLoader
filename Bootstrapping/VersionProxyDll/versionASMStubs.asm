.DATA
g_FunctionTable QWORD 17 dup(?)
PUBLIC g_FunctionTable

.CODE
GetFileVersionInfoA PROC
	jmp [g_FunctionTable + 0 * 8]
GetFileVersionInfoA ENDP

GetFileVersionInfoByHandle PROC
	jmp [g_FunctionTable + 1 * 8]
GetFileVersionInfoByHandle ENDP

GetFileVersionInfoExA PROC
	jmp [g_FunctionTable + 2 * 8]
GetFileVersionInfoExA ENDP

GetFileVersionInfoExW PROC
	jmp [g_FunctionTable + 3 * 8]
GetFileVersionInfoExW ENDP

GetFileVersionInfoSizeA PROC
	jmp [g_FunctionTable + 4 * 8]
GetFileVersionInfoSizeA ENDP

GetFileVersionInfoSizeExA PROC
	jmp [g_FunctionTable + 5 * 8]
GetFileVersionInfoSizeExA ENDP

GetFileVersionInfoSizeExW PROC
	jmp [g_FunctionTable + 6 * 8]
GetFileVersionInfoSizeExW ENDP

GetFileVersionInfoSizeW PROC
	jmp [g_FunctionTable + 7 * 8]
GetFileVersionInfoSizeW ENDP

GetFileVersionInfoW PROC
	jmp [g_FunctionTable + 8 * 8]
GetFileVersionInfoW ENDP

VerFindFileA PROC
	jmp [g_FunctionTable + 9 * 8]
VerFindFileA ENDP

VerFindFileW PROC
	jmp [g_FunctionTable + 10 * 8]
VerFindFileW ENDP

VerInstallFileA PROC
	jmp [g_FunctionTable + 11 * 8]
VerInstallFileA ENDP

VerInstallFileW PROC
	jmp [g_FunctionTable + 12 * 8]
VerInstallFileW ENDP

VerLanguageNameA PROC
	jmp [g_FunctionTable + 13 * 8]
VerLanguageNameA ENDP

VerLanguageNameW PROC
	jmp [g_FunctionTable + 14 * 8]
VerLanguageNameW ENDP

VerQueryValueA PROC
	jmp [g_FunctionTable + 15 * 8]
VerQueryValueA ENDP

VerQueryValueW PROC
	jmp [g_FunctionTable + 16 * 8]
VerQueryValueW ENDP

END
