using System.Runtime.InteropServices;

using ClangSharp.Interop;

using Serilog;

using UnrealCoreObjectApiSourceGen.Models;

namespace UnrealCoreObjectApiSourceGen.Services.SourceGen {
    public unsafe class NativeCodeParser : INativeCodeParser {
        private readonly ILogger _logger;

        public NativeCodeParser(ILogger logger) {
            _logger = logger;
        }

        public unsafe bool ParseSourceFile(string file, out LibClangTranslationRecord? libClangTranslationRecord) {
            libClangTranslationRecord = null!;

            var index = clang.createIndex(0, 1);
            var pTranslationUnit = CreateTranslationUnit(file, index);

            if (pTranslationUnit is null) {
                _logger.Error("Could not parse the file!");
                return false;
            }

            var cursor = clang.getTranslationUnitCursor(pTranslationUnit);
            libClangTranslationRecord = new(cursor, pTranslationUnit, index);
            return true;
        }

        private static CXTranslationUnitImpl* CreateTranslationUnit(string file, void* index) {
            var managedCommandLineArgs = new List<string>();
            var moreIncludeFolder = new List<string>();
            foreach (var folder in Directory.GetDirectories(Path.Combine(Environment.CurrentDirectory, "Source", "Runtime"))) {
                var subFolders = Directory.GetDirectories(folder);
                if (subFolders.FirstOrDefault(i => i.EndsWith("Public")) is string publicFolder) {
                    moreIncludeFolder.Add($"-I{publicFolder}");
                }
            }
            managedCommandLineArgs.AddRange(moreIncludeFolder);
            managedCommandLineArgs.AddRange([
                "-xc++",
                "-E",
                "--comments",
                "-Wmacro-redefined",
                "-frelaxed-template-template-args",
                "-fms-compatibility",
                "-ferror-limit=1",
                "-Wunused-command-line-argument",

                "-DUE_BUILD_DEVELOPMENT=1",
                "-DUE_BUILD_MINIMAL=1",
                "-DWITH_EDITOR=0",
                "-DWITH_EDITORONLY_DATA=1",
                "-DWITH_SERVER_CODE=1",
                "-DWITH_ENGINE=0",
                "-DWITH_UNREAL_DEVELOPER_TOOLS=0",
                "-DWITH_PLUGIN_SUPPORT=0",
                "-DIS_MONOLITHIC=1",
                "-DIS_PROGRAM=1",
                "-DPLATFORM_WINDOWS=1",
                "-DCORE_API=",
                "-DCOREUOBJECT_API=",
                "-DDATASMITHEXPORTER_API=",
                "-DDATASMITHCORE_API=",
                "-DDIRECTLINK_API=",
                "-DWIN32=1",
                "-D_WIN32_WINNT=0x0601",
                "-DWINVER=0x0601",
                "-DUNICODE",
                "-D_UNICODE",
                "-DUE_BUILD_DEVELOPMENT_WITH_DEBUGGAME=0",
                "-DUBT_COMPILED_PLATFORM=Windows"
            ]);

            AllocateNativeArguments(file, managedCommandLineArgs, out var ppCommandLineArgs, out var pArgumentString);

            var pTranslationUnit = clang.parseTranslationUnit(index, pArgumentString, ppCommandLineArgs,
                managedCommandLineArgs.Count, null, 0, (uint)(CXTranslationUnit_Flags.CXTranslationUnit_CXXChainedPCH | CXTranslationUnit_Flags.CXTranslationUnit_KeepGoing));

            FreeArguments(managedCommandLineArgs, ppCommandLineArgs, pArgumentString);
            return pTranslationUnit;
        }

        private static void AllocateNativeArguments(string file, List<string> managedCommandLineArgs, out sbyte** ppCommandLineArgs, out sbyte* argumentString) {
            ppCommandLineArgs = (sbyte**)Marshal.AllocCoTaskMem(sizeof(nint) * managedCommandLineArgs.Count);
            for (var x = 0; x < managedCommandLineArgs.Count; x++) {
                ppCommandLineArgs[x] = (sbyte*)Marshal.StringToCoTaskMemUTF8(managedCommandLineArgs[x]);
            }
            argumentString = (sbyte*)Marshal.StringToCoTaskMemUTF8(file);
        }

        private static void FreeArguments(List<string> managedCommandLineArgs, sbyte** ppCommandLineArgs, sbyte* argumentString) {
            Marshal.FreeCoTaskMem((nint)argumentString);
            for (var x = 0; x < managedCommandLineArgs.Count; x++) {
                Marshal.FreeCoTaskMem((nint)ppCommandLineArgs[x]);
            }
            Marshal.FreeCoTaskMem((nint)ppCommandLineArgs);
        }
    }
}
