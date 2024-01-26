//Loads the CLR Host C# Assemblies

#include "CLR.hpp"

#include "coreclrhost.h"

//directory where the CLR runtime 
#define PAL_MOD_MANAGER_CORE_CLR_DIR "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App"

//the version to check for
#define PAL_MOD_MANAGER_CORE_CLR_VER_DIR "8.0.1"

//makes Trust Platform Assembled List
static inline void BuildTpaList(char* directory, std::string& tpaList)
{
	std::string dir = std::string(directory) + "\\*.dll";

	WIN32_FIND_DATAA findData;
	HANDLE fileHandle = FindFirstFileA(dir.c_str(), &findData);

	//generate a semi-colon seperate string list
	if (fileHandle != INVALID_HANDLE_VALUE)
	{
		//while (FindNextFileA(fileHandle, &findData))
		//{
		//	tpaList += directory + "\\" + findData.cFileName + ";";
		//}

		do
		{
			tpaList.append(directory);
			tpaList.append("\\");
			tpaList.append(findData.cFileName);
			tpaList.append(";");
		} while (FindNextFileA(fileHandle, &findData));

		FindClose(fileHandle);
	}
}

//makes Trust Platform Assembled List
static inline void BuildTpaList(std::string& directory, std::string& tpaList)
{
	auto searchPath = directory + "\\*.dll";

	WIN32_FIND_DATAA findData;
	HANDLE fileHandle = FindFirstFileA(searchPath.c_str(), &findData);

	//generate a semi-colon seperate string list
	if (fileHandle != INVALID_HANDLE_VALUE)
	{
		while (FindNextFileA(fileHandle, &findData))
		{
			tpaList += directory + "\\" + findData.cFileName + ";";
		}
		FindClose(fileHandle);
	}
}

//creates the pointer to the Mod DLL
PalModManager::CLR::managed_direct_method_ptr PalModManager::CLR::CLRHost::CreateManagedDelegate(const char* managedAssemblyName, const char* managedClassName, const char* managedsubroutineName)
{
	auto createManagedDelegate = (coreclr_create_delegate_ptr)GetProcAddress(_coreClr, "coreclr_create_delegate");

	if (createManagedDelegate == NULL) {
		printf("'coreclr_create_delegate' could not be found within coreclr.dll!\n");
		return NULL;
	}

	// The CLR finds the assembly from the TPA list we created it with.
	managed_direct_method_ptr managedSubroutine;
	printf(managedAssemblyName);
	printf("\n");

	printf(managedClassName);
	printf("\n");

	printf(managedsubroutineName);
	printf("\n");

	int hResponse = createManagedDelegate(_hostHandle, _domainId,
		managedAssemblyName,
		managedClassName,
		managedsubroutineName,
		(void**)&managedSubroutine);

	if (hResponse >= 0) {
		printf("Created Managed Delegate to: '%s::%s'\n", managedClassName, managedsubroutineName);
		return managedSubroutine;
	}

	printf("'coreclr_create_delegate' failed to create the delegate with error code: %x\n", hResponse);

	getchar();
	return NULL;
}

//load a managed C# Assemvly
bool PalModManager::CLR::CLRHost::LoadManagedAssembly(const char* filePath, const char* appDomainId, const char* managedAssemblyName, const char* entryClassName, const char* entrySubroutine)
{
	char runtimeDirectory[MAX_PATH];
	//std::string runtimeDirectory; runtimeDirectory.resize(MAX_PATH);

	GetFullPathNameA(filePath, MAX_PATH, runtimeDirectory, NULL);

	printf(runtimeDirectory);

	std::string coreCLRDir = std::string(std::string(PAL_MOD_MANAGER_CORE_CLR_DIR) + "\\" + std::string(PAL_MOD_MANAGER_CORE_CLR_VER_DIR));
	std::string coreClrAssembly = std::string(std::string(PAL_MOD_MANAGER_CORE_CLR_DIR) + "\\" + std::string(PAL_MOD_MANAGER_CORE_CLR_VER_DIR) + std::string("\\coreclr.dll"));

	printf("Full Assembly Name: %s\n", managedAssemblyName);
	printf("Loading Directory: %s\n", runtimeDirectory);
	printf("Loading Runtime: %s\n", coreClrAssembly.c_str());

	_coreClr = LoadLibraryExA(coreClrAssembly.c_str(), NULL, 0);

	if (_coreClr == NULL) {
		printf("There was an error loading coreclr.dll!\n");
		return false;
	}

	printf("Found and loaded coreclr.dll\n");

	auto initializeCoreClr = (coreclr_initialize_ptr)GetProcAddress(_coreClr, "coreclr_initialize");

	if (initializeCoreClr == NULL) {
		printf("Could not find 'coreclr_initialize' subroutine in coreclr.dll!\n");
		return false;
	}

	printf("Building Trusted Platform Assemblies list...\n");

	std::string tpaList = "", dir = coreCLRDir;
	BuildTpaList(dir, tpaList);
	BuildTpaList(runtimeDirectory, tpaList);

	printf("Trusted Platform Assemblies list complete.\n");

	const char* propertyKeys[] = { "TRUSTED_PLATFORM_ASSEMBLIES", "APP_CONTEXT_BASE_DIRECTORY" };
	const char* propertyValues[] = { tpaList.c_str(), runtimeDirectory };

	int hResponse = initializeCoreClr(
		coreCLRDir.c_str(),
		appDomainId,
		sizeof(propertyKeys) / sizeof(char*),
		propertyKeys,
		propertyValues,
		&_hostHandle,
		&_domainId);

	if (hResponse >= 0) {
		printf("CoreClr has started!\n");
		_managedDirectMethod = CreateManagedDelegate(managedAssemblyName, entryClassName, entrySubroutine);
		_managedDirectMethod();

		return true;
	}

	printf("CoreClr 'initializeCoreClr' has failed with response code: %i\n", hResponse);

	return true;
}