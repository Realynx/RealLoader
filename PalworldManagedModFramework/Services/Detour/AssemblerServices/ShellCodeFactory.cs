using Iced.Intel;

using PalworldManagedModFramework.Services.Detour.AssemblerServices.Interfaces;

using static Iced.Intel.AssemblerRegisters;

namespace PalworldManagedModFramework.Services.Detour.AssemblerServices {
    public class ShellCodeFactory : IShellCodeFactory {
        /// <summary>
        /// push low32
        /// mov [rsp+4], hi32
        /// ret
        /// </summary>
        /// <param name="redirect"></param>
        /// <returns></returns>
        public byte[] BuildStackDetour64(nint redirect) {
            var codeWriter = new CodeWriterImpl();
            var assembler = new Assembler(64);

            var lo32 = (uint)(redirect & 0xFFFFFFFF);
            var hi32 = (uint)((redirect >> 32) & 0xFFFFFFFF);

            assembler.push(lo32);
            assembler.mov(__dword_ptr[rsp + 4], hi32);
            assembler.ret();

            // Encode the instructions
            assembler.Assemble(codeWriter, 0);
            var bytes = codeWriter.ToArray();
            return bytes;
        }

        /// <summary>
        /// Orignial Instructions (Usually function prologue)
        /// Stack detour to original func + (Orignial Instructions Length).
        /// </summary>
        /// <param name="overwrittenCodes"></param>
        /// <param name="offsetStackAddr"></param>
        /// <returns></returns>
        public byte[] BuildTrampoline64(byte[] overwrittenCodes, nint offsetStackAddr) {
            var finalDetour = BuildStackDetour64(offsetStackAddr);
            var trampolineCodes = new byte[overwrittenCodes.Length + finalDetour.Length];

            overwrittenCodes.CopyTo(trampolineCodes, 0);
            finalDetour.CopyTo(trampolineCodes, overwrittenCodes.Length);
            return trampolineCodes;
        }
    }
}
