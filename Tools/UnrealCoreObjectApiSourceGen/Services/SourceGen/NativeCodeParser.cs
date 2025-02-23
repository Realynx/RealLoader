﻿using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using ClangSharp.Interop;

using Serilog;

namespace UnrealCoreObjectApiSourceGen.Services.SourceGen {
    public unsafe class NativeCodeParser {
        private readonly ILogger _logger;

        public NativeCodeParser(ILogger logger) {
            _logger = logger;
        }

        public unsafe bool ParseSourceFile(string file) {
            var index = clang.createIndex(0, 1);

            var managedCommandLineArgs = new List<string>();
            foreach (var folder in Directory.GetDirectories(Environment.CurrentDirectory)) {
                var subFolders = Directory.GetDirectories(folder);
                if (subFolders.FirstOrDefault(i => i.EndsWith("Public")) is string publicFolder) {
                    managedCommandLineArgs.Add($"-I{publicFolder}");
                }
            }

            managedCommandLineArgs.AddRange([
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
                "-DCORE_API=DLLIMPORT",
                "-DCOREUOBJECT_API=DLLIMPORT",
                "-DDATASMITHEXPORTER_API=DLLIMPORT",
                "-DDATASMITHCORE_API=DLLIMPORT",
                "-DDIRECTLINK_API=DLLIMPORT",
                "-DWIN32=1",
                "-D_WIN32_WINNT=0x0601",
                "-DWINVER=0x0601",
                "-DUE_BUILD_DEVELOPMENT_WITH_DEBUGGAME=0",
                "-DUBT_COMPILED_PLATFORM=Windows"
            ]);

            var ppCommandLineArgs = (nint**)Marshal.AllocCoTaskMem(sizeof(nint) * managedCommandLineArgs.Count);
            for (var x = 0; x < managedCommandLineArgs.Count; x++) {
                ppCommandLineArgs[x] = (nint*)Marshal.StringToCoTaskMemUTF8(managedCommandLineArgs[x]);
            }
            var argumentString = Marshal.StringToCoTaskMemUTF8(file);

            CXTranslationUnitImpl* translationUnit = clang.parseTranslationUnit(index, (sbyte*)argumentString, (sbyte**)ppCommandLineArgs, managedCommandLineArgs.Count, null, 0, (uint)CXTranslationUnit_Flags.CXTranslationUnit_SingleFileParse);
            Marshal.FreeCoTaskMem(argumentString);

            for (var x = 0; x < managedCommandLineArgs.Count; x++) {
                Marshal.FreeCoTaskMem((nint)ppCommandLineArgs[x]);
            }
            Marshal.FreeCoTaskMem((nint)ppCommandLineArgs);

            if (translationUnit is null) {
                _logger.Error("Could not parse the file!");
                return false;
            }

            var cursor = clang.getTranslationUnitCursor(translationUnit);

            var nodeVisitFuncPtr = (delegate* unmanaged[Cdecl]<CXCursor, CXCursor, void*, CXChildVisitResult>)&NodeVisit;
            _ = clang.visitChildren(cursor, nodeVisitFuncPtr, null);

            clang.disposeTranslationUnit(translationUnit);
            clang.disposeIndex(index);
            return true;
        }


        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        private static CXChildVisitResult NodeVisit(CXCursor cursor, CXCursor parent, void* clientData) {
            var displayName = clang.getCursorDisplayName(cursor).ToString();


            //if (displayName != "COREUOBJECT_API") {
            //    return CXChildVisitResult.CXChildVisit_Continue;
            //}

            //if (cursor.Kind != CXCursorKind.CXCursor_ClassTemplate) {
            //    return CXChildVisitResult.CXChildVisit_Continue;
            //}

            //if (cursor.DeclKind is CX_DeclKind.CX_DeclKind_Var) {
            //    Console.WriteLine();
            //    Console.WriteLine($"Node: {spelling}: Node Type: {cursor.DeclKind}");
            //    return CXChildVisitResult.CXChildVisit_Recurse;
            //}
            var location = clang.getCursorLocation(cursor);
            location.GetFileLocation(out var file, out var line, out var column, out var _);

            var printingPolicy = clang.getCursorPrintingPolicy(cursor);
            var prettyPrint = clang.getCursorPrettyPrinted(cursor, printingPolicy);
            Console.WriteLine(prettyPrint);
            // Console.WriteLine(USR);

            var spelling = clang.getCursorSpelling(cursor).ToString();
            //Console.WriteLine($"Node: {spelling}: Node Type: {cursor.Kind}, Display Name: {displayName}");
            //Console.WriteLine($"{Path.GetFileName(file.Name.CString)}::{spelling} - ({line},{column}), {cursor.Kind}");

            //Console.WriteLine($"({cursor.NominatedBaseClass}){cursor.DisplayName}");
            // Console.WriteLine($"    children: {cursor.NumChildren}");
            return CXChildVisitResult.CXChildVisit_Recurse;
        }
    }
}
