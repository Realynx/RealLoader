REM compiles all the dist for C# and C++

msbuild RealLoader.sln /p:Configuration=Dist /p:Platform=x64 -target:Rebuild
dotnet publish RealLoader.sln /p:Configuration=Release /p:PublishProfile=Release
pause