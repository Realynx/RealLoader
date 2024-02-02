workspace "C++WindowsBuild"
architecture "x64"
startproject "Bootstrapper"

    configurations
{
    "Debug",
    "Release",
    "Dist"
}

outputdir = "%{cfg.buildcfg}-%{cfg.system}-%{cfg.architecture}"

--CLRHost for injecting
project "CLRHost"
location "CLRHost"
kind "SharedLib"
language "C++"
            
--targetdir ("bin/" .. outputdir .. "/%{prj.name}")
targetdir("GithubSymbols\\Windows")
objdir ("bin-obj/" .. outputdir .. "/%{prj.name}")
    
    
files 
{
    "CLRHost\\src\\**.h",
    "CLRHost\\src\\**.c",
    "CLRHost\\src\\**.hpp",
    "CLRHost\\src\\**.cpp",
}

includedirs
{
    "CLRHost\\src",
    "Common\\Cpp"
}

links
{
    "C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Host.win-x64\\8.0.1\\runtimes\\win-x64\\native\\nethost.lib"
}
    
flags
{
    --"LinkTimeOptimization",
    "NoRuntimeChecks",
    "MultiProcessorCompile"
}

--platforms
filter "system:windows"
cppdialect "C++17"
staticruntime "On"
systemversion "latest"

defines
{
    "_WIN32"
}

--configs
filter "configurations:Debug"
defines "SMOK_DEBUG"
symbols "On"

filter "configurations:Release"
defines "SMOK_RELEASE"
optimize "On"

filter "configurations:Dist"
defines "SMOK_DIST"
optimize "On"

--Bootstrapper
project "Bootstrapper"
location "PalWorldManagedModFramework.EntryPoint"
kind "ConsoleApp"
language "C++"
            
--targetdir ("bin/" .. outputdir .. "/%{prj.name}")
targetdir("GithubSymbols\\Windows")
objdir ("bin-obj/" .. outputdir .. "/%{prj.name}")
    
    
files 
{
    "PalWorldManagedModFramework.EntryPoint\\src\\**.h",
    "PalWorldManagedModFramework.EntryPoint\\src\\**.c",
    "PalWorldManagedModFramework.EntryPoint\\src\\**.hpp",
    "PalWorldManagedModFramework.EntryPoint\\src\\**.cpp",
}

includedirs
{
    "PalWorldManagedModFramework.EntryPoint\\src",
    "Common\\Cpp"
}

links
{

}
   
flags
{
    "NoRuntimeChecks",
    "MultiProcessorCompile"
}

--platforms
filter "system:windows"
cppdialect "C++17"
staticruntime "On"
systemversion "latest"

defines
{
    "_WIN32"
}

--configs
filter "configurations:Debug"
defines "SMOK_DEBUG"
symbols "On"

filter "configurations:Release"
defines "SMOK_RELEASE"
optimize "On"

filter "configurations:Dist"
defines "SMOK_DIST"
optimize "On"

defines
{
    "NDEBUG"
}