REM compiles all the dist for C# and C++

msbuild C++WindowsBuild.sln /p:Configuration=Dist /p:Platform=x64 -target:Rebuild
dotnet publish PalworldManagedModFramework.sln /p:Configuration=Release