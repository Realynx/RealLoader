/*
Bootstrapper for hyjacking Unreal Engine's normal lilly-padding of EXEs. Injects payload into Pal.
*/

//include a stripped windows.h
#define WIN32_LEAN_AND_MEAN
#include <windows.h>

//OS Processes
#include <winnt.h>
#include <TlHelp32.h>

//custom string for thick and C char arrays
#include <String.hpp>

//output and filesystem
#include <iostream>
#include <filesystem>

//the entry point
#if defined(_WIN32)
int __cdecl wmain(int argc, wchar_t* argv[])
#else
int main(int argc, char* argv[])
#endif
{
	//if no args
	if (!argc)
	{
		std::cout << "No Args were found :(, we can't run the boot strapper!\n";
		getchar();
		return -1;
	}
	std::string path = PalMM::Util::ConvertThickStringToCString(argv[0]); //get the path to Pal
	bool modsEnabled = (argc > 1 ? true : false); //known bug, this is true regardless for some reason, we tried strings with a "-modded" flag but it wasn't working either

	// gets the path of the real game, and runs it with the needed arguments
	std::filesystem::path palPath = path;
	palPath = palPath.replace_filename("Game-Palworld-Win64-Shipping").string() + ".exe";

	//starts Pal Worls
	PROCESS_INFORMATION PI; ZeroMemory(&PI, sizeof(PI)); // Null the memory
	STARTUPINFOA SI; ZeroMemory(&SI, sizeof(SI)); // Null the memory
	if (!CreateProcessA(palPath.string().c_str(), NULL, NULL, NULL, FALSE,
		DETACHED_PROCESS, NULL, NULL, &SI, &PI))
	{
		printf("Failed to create a detached process at \"%s\"\n", palPath.string().c_str());
		getchar();
		return -1;
	}

	//enables modding if the flag is active
	if (modsEnabled) //known bug, this is true regardless for some reason, we tried strings with a "-modded" flag but it wasn't working either
	{
		//gets it's process
		PI.hProcess = OpenProcess(PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ, FALSE, PI.dwProcessId);

		//writes the payload into the Pal process
		const char* dllPath = "CLRHost.dll";
		LPVOID pDllPath = VirtualAllocEx(PI.hProcess, 0, strlen(dllPath) + 1, MEM_COMMIT, PAGE_READWRITE);
		WriteProcessMemory(PI.hProcess, pDllPath, (LPVOID)dllPath, strlen(dllPath) + 1, 0);

		//load the payload and execute a remote thread in Pal
		LPVOID pLoadLibrary = GetProcAddress(GetModuleHandle(L"kernel32.dll"), "LoadLibraryA");
		HANDLE hThread = CreateRemoteThread(PI.hProcess, NULL, 0, (LPTHREAD_START_ROUTINE)pLoadLibrary, pDllPath, 0, NULL);
	}

	return 0;
}