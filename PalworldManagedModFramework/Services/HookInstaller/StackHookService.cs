
using Iced.Intel;

using Microsoft.Extensions.Logging;

namespace PalworldManagedModFramework.Services.HookInstaller {
    public class StackHookService {
        private readonly ILogger _logger;

        public StackHookService(ILogger logger) {
            _logger = logger;
        }

        //public void InstallHook(nint hookAddress, nint redirect) {
        //    var codeWriter = new CodeWriterImpl();
        //    var encoder = Encoder.Create(64, codeWriter); // Assuming x64

        //    encoder.Encode(Instruction.Create(Opcode.Jmp_m64, new MemoryOperand(Register.RIP, 0)), hookAddress.ToInt64());
        //    codeWriter.WriteInt64(redirect.ToInt64());
        //}
    }
}
