
using PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs;

namespace PalworldManagedModFramework.Sdk.Services.UnrealHook {
    public unsafe record UnrealEventRecord {
        public delegate void ExecuteOriginalCallback(ref UObject target, ref UFunction function, ref object parameter);
        public UnrealEventRecord(ref UObject instance, ref UFunction uFunction, ref object voidPtr, ExecuteOriginalCallback executeOriginal) {
            Instance = instance;
            UFunction = uFunction;
            VoidPtr = voidPtr;
            ExecuteOriginal = executeOriginal;
        }


        public UObject Instance { get; }
        public UFunction UFunction { get; }
        public object VoidPtr { get; }
        public ExecuteOriginalCallback ExecuteOriginal { get; }
    }
}
