// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include <iostream> // Standard C++ library for console I/O
#include <fstream>
#include <string> // Standard C++ Library for string manip

#include <Windows.h> // WinAPI Header
#include <winnt.h>
#include <TlHelp32.h> //WinAPI Process API

// Standard headers
#include <stdio.h>
#include <stdint.h>
#include <stdlib.h>
#include <string.h>
#include <iostream>
#include <vector>
#include <filesystem>
#include "atlbase.h"
#include "atlstr.h"
#include "comutil.h"
#include <thread>

//
//
//#include <CLR/CLR.hpp>
//
//static inline void CLRRun(CLR::CLRHost* host, const std::basic_string<char_t>& app_path)
//{
//	std::this_thread::sleep_for(std::chrono::seconds(2));
//
//	// Load .NET Core
//	std::vector<const char_t*> args{ app_path.c_str() };
//	int rc = host->init_for_cmd_line_fptr(args.size(), args.data(), nullptr, &host->cxt);
//
//	//run the CLR
//	host->run_app_fptr(host->cxt);
//	
//	//clean up CLR when done
//	host->close_fptr(host->cxt);
//}
//
////the main bulk of code for the actually running of DLLs and such
//int run_app_example(CLR::CLRHost* host, const std::basic_string<char_t>& root_path)
//{
//	const std::basic_string<char_t> app_path = root_path + STR("ManagedModFramework\\PalworldManagedModFramework.dll");
//
//	//init the function pointers
//	if (!host->Init(app_path.c_str()))
//	{
//		assert(false && "Failure: load_hostfxr()");
//		return EXIT_FAILURE;
//	}
//
//	// Load .NET Core
//	//std::vector<const char_t*> args{ app_path.c_str() };
//	//int rc = host->init_for_cmd_line_fptr(args.size(), args.data(), nullptr, &host->cxt);
//	//if (rc != 0 || host->cxt == nullptr)
//	//{
//	//	std::cerr << "Init failed: " << std::hex << std::showbase << rc << std::endl;
//	//	host->close_fptr(host->cxt);
//	//	return EXIT_FAILURE;
//	//}
//
//	// Create and start the thread
//	std::thread clrThread(CLRRun, host, app_path);
//
//	clrThread.detach();
//
//	//host->run_app_fptr(host->cxt);
//	
//
//	return EXIT_SUCCESS;
//}

//// use this if you want to read the executable from disk
//HANDLE MapFileToMemory(LPCSTR filename)
//{
//	std::streampos size;
//	std::fstream file;
//	file = std::fstream(filename, std::ios::in | std::ios::binary | std::ios::ate);
//
//	if (file.is_open())
//	{
//		size = file.tellg();
//
//		char* Memblock = new char[size]();
//
//		file.seekg(0, std::ios::beg);
//		file.read(Memblock, size);
//		file.close();
//
//		return Memblock;
//	}
//
//	printf("Failed to find file at \"%s\"\n", filename);
//	getchar();
//	return 0;
//}

//int RunPortableExecutable(void* Image)
//{
//	IMAGE_DOS_HEADER* DOSHeader; // For Nt DOS Header symbols
//	IMAGE_NT_HEADERS* NtHeader; // For Nt PE Header objects & symbols
//	IMAGE_SECTION_HEADER* SectionHeader;
//
//	PROCESS_INFORMATION PI;
//	STARTUPINFOA SI;
//
//	CONTEXT* CTX;
//
//	DWORD* ImageBase; //Base address of the image
//	void* pImageBase; // Pointer to the image base
//
//	int count;
//	char CurrentFilePath[1024];
//
//	DOSHeader = PIMAGE_DOS_HEADER(Image); // Initialize Variable
//	NtHeader = PIMAGE_NT_HEADERS(DWORD(Image) + DOSHeader->e_lfanew); // Initialize
//
//	GetModuleFileNameA(0, CurrentFilePath, 1024); // path to current executable
//
//	if (NtHeader->Signature == IMAGE_NT_SIGNATURE) // Check if image is a PE File.
//	{
//		ZeroMemory(&PI, sizeof(PI)); // Null the memory
//		ZeroMemory(&SI, sizeof(SI)); // Null the memory
//
//		printf("Signeture found!\n");
//
//		if (CreateProcessA(CurrentFilePath, NULL, NULL, NULL, FALSE,
//			CREATE_SUSPENDED, NULL, NULL, &SI, &PI)) // Create a new instance of current
//			//process in suspended state, for the new image.
//		{
//			// Allocate memory for the context.
//			CTX = LPCONTEXT(VirtualAlloc(NULL, sizeof(CTX), MEM_COMMIT, PAGE_READWRITE));
//			CTX->ContextFlags = CONTEXT_FULL; // Context is allocated
//
//			if (GetThreadContext(PI.hThread, LPCONTEXT(CTX))) //if context is in thread
//			{
//				// Read instructions
//				ReadProcessMemory(PI.hProcess, LPCVOID(CTX->Rbx +8), LPVOID(&ImageBase), 4, 0);
//
//				pImageBase = VirtualAllocEx(PI.hProcess, LPVOID(NtHeader->OptionalHeader.ImageBase),
//					NtHeader->OptionalHeader.SizeOfImage, 0x3000, PAGE_EXECUTE_READWRITE);
//
//				// Write the image to the process
//				WriteProcessMemory(PI.hProcess, pImageBase, Image, NtHeader->OptionalHeader.SizeOfHeaders, NULL);
//
//				for (count = 0; count < NtHeader->FileHeader.NumberOfSections; count++)
//				{
//					SectionHeader = PIMAGE_SECTION_HEADER(DWORD(Image) + DOSHeader->e_lfanew + 248 + (count * 40));
//
//					WriteProcessMemory(PI.hProcess, LPVOID(DWORD(pImageBase) + SectionHeader->VirtualAddress),
//						LPVOID(DWORD(Image) + SectionHeader->PointerToRawData), SectionHeader->SizeOfRawData, 0);
//				}
//				WriteProcessMemory(PI.hProcess, LPVOID(CTX->Rbx + 8),
//					LPVOID(&NtHeader->OptionalHeader.ImageBase), 4, 0);
//
//				// Move address of entry point to the eax register
//				CTX->Rax = DWORD(pImageBase) + NtHeader->OptionalHeader.AddressOfEntryPoint;
//				SetThreadContext(PI.hThread, LPCONTEXT(CTX)); // Set the context
//				ResumeThread(PI.hThread); //´Start the process/call main()
//
//				return 0; // Operation was successful.
//			}
//		}
//	}
//}

//the entry point
#if defined(Window_Build)
int __cdecl wmain(int argc, wchar_t* argv[])
#else
int main(int argc, char* argv[])
#endif
{
	//get args if they exist
	std::string path = "", modFlag = "-modded";
	if (argc > 0)
	{
		//const wchar_t* a = (wchar_t*)"C:\\Program Files (x86)\\Steam\\steamapps\\common\\Palworld\\Pal\\Binaries\\Win64\\Palworld-Win64-Shipping.exe";
		path = std::string(CStringA(argv[0]).GetString());
	}
	//printf(""path.c_str());

	// gets the path of the real game, and runs it with the needed arguments
	std::filesystem::path absolutePath = path;
	absolutePath = absolutePath.replace_filename("Game-Palworld-Win64-Shipping").string() + ".exe";

	std::string command = /*std::string("\"") +*/ absolutePath.string(); /*+ std::string("\"");*/
	printf("%s\n", command.c_str());

	//starts Pal World on a remote thread
	PROCESS_INFORMATION PI; ZeroMemory(&PI, sizeof(PI)); // Null the memory
	STARTUPINFOA SI; ZeroMemory(&SI, sizeof(SI)); // Null the memory
	if (!CreateProcessA(command.c_str(), NULL, NULL, NULL, FALSE,
		DETACHED_PROCESS /*CREATE_SUSPENDED*/, NULL, NULL, &SI, &PI))
	{
		printf("Failed to create a suspended process at \"%\"\n", command.c_str());
		getchar();
		return -1;
	}

	PI.hProcess = OpenProcess(PROCESS_VM_READ | PROCESS_VM_WRITE | PROCESS_VM_OPERATION, FALSE, PI.dwProcessId);

	const char* dllPath = "CLRHost.dll";
	LPVOID pDllPath = VirtualAllocEx(PI.hProcess, 0, strlen(dllPath) + 1, MEM_COMMIT, PAGE_READWRITE);
	WriteProcessMemory(PI.hProcess, pDllPath, (LPVOID)dllPath, strlen(dllPath) + 1, 0);

	LPVOID pLoadLibrary = GetProcAddress(GetModuleHandle(L"kernel32.dll"), "LoadLibraryA");
	HANDLE hThread = CreateRemoteThread(PI.hProcess, NULL, 0, (LPTHREAD_START_ROUTINE)pLoadLibrary, pDllPath, 0, NULL);

	// Wait for the remote thread to complete
	WaitForSingleObject(hThread, INFINITE);
	getchar();


	const char* functionName = "LoadCLRHost";
	LPVOID pFunctionName = VirtualAllocEx(PI.hProcess, 0, strlen(functionName) + 1, MEM_COMMIT, PAGE_READWRITE);
	WriteProcessMemory(PI.hProcess, pFunctionName, (LPVOID)functionName, strlen(functionName) + 1, 0);

	LPVOID pGetProcAddress = GetProcAddress(GetModuleHandle(L"kernel32.dll"), "GetProcAddress");
	hThread = CreateRemoteThread(PI.hProcess, NULL, 0, (LPTHREAD_START_ROUTINE)pGetProcAddress, pFunctionName, 0, NULL);

	if (hThread == NULL) {
		std::cerr << "Failed to create remote thread" << std::endl;
		return 1;
	}

	// Wait for the remote thread to complete
	WaitForSingleObject(hThread, INFINITE);
	getchar();

	// Get the context of the thread
	CONTEXT ctx;
	ctx.ContextFlags = CONTEXT_FULL; // Retrieves all registers

	if (GetThreadContext(hThread, &ctx)) {
		// Successfully retrieved the thread context
		std::cout << "RAX: " << ctx.Rax << std::endl;
	}
	else {
		std::cerr << "Failed to get thread context" << std::endl;
	}

	DWORD64 exportedAddresss = ctx.Rax;
	CloseHandle(hThread);

	hThread = CreateRemoteThread(PI.hProcess, NULL, 0, (LPTHREAD_START_ROUTINE)exportedAddresss, NULL, 0, NULL);

	getchar();

	return 0;
}