namespace PalworldManagedModFramework.PalWorldSdk.Services {
    public static class UnrealHook {
        public static void HookFunction<T>(string functionName, Func<T, T> Callback, bool trampoline = false) {

        }

        public static void HookFunction<T>(string functionName, Action<T> Callback, bool trampoline = false) {

        }
    }
}
