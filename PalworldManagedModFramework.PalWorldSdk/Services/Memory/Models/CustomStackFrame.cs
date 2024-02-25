using System.Diagnostics;

namespace PalworldManagedModFramework.Sdk.Services.Memory.Models {
    public class CustomStackFrame {
        public nint StackAddress { get; set; }
        public nint CallerAddress { get; set; }

        public ProcessModule? Module {
            get {
                return Process.GetCurrentProcess()
                    .Modules
                    .Cast<ProcessModule>()
                    .FirstOrDefault(i => CallerAddress >= i.BaseAddress && CallerAddress <= i.BaseAddress + i.ModuleMemorySize);
            }
        }
    }
}
