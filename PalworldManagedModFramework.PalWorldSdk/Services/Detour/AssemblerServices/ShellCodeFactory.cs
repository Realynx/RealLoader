using System;

using Iced.Intel;

using PalworldManagedModFramework.Sdk.Services.Detour.AssemblerServices.Interfaces;

using static Iced.Intel.AssemblerRegisters;

namespace PalworldManagedModFramework.Sdk.Services.Detour.AssemblerServices {
    public class ShellCodeFactory : IShellCodeFactory {
        /// <summary>
        /// push low32
        /// mov [rsp+4], hi32
        /// ret
        /// </summary>
        /// <param name="redirect"></param>
        /// <returns></returns>
        public byte[] BuildStackDetour64(nint redirect) {
            // Jmp [RIP + 0]
            // Address bytes
            var jmpBytes = new byte[] {
                0xff,0x25,0x00,0x00,0x00,0x00
            };

            var returnBytes = new byte[jmpBytes.Length + 8];
            jmpBytes.CopyTo(returnBytes, 0);
            BitConverter.GetBytes(redirect).CopyTo(returnBytes, jmpBytes.Length);

            return returnBytes;

            //var codeWriter = new CodeWriterImpl();
            //var assembler = new Assembler(64);

            ////var lo32 = (uint)(redirect & 0xFFFFFFFF);
            ////var hi32 = (uint)(redirect >> 32 & 0xFFFFFFFF);

            //var part1 = (ushort)(redirect & 0xFFFF);        // Lowest 16 bits
            //var part2 = (ushort)((redirect >> 16) & 0xFFFF); // Next 16 bits
            //var part3 = (ushort)((redirect >> 32) & 0xFFFF); // Next 16 bits
            //var part4 = (ushort)((redirect >> 48) & 0xFFFF); // Highest 16 bits

            //assembler.push(part1);
            //assembler.push(part2);
            //assembler.push(part3);
            //assembler.push(part4);



            //assembler.jmp(AssemblerMemoryOperand.RIPRelative(0));
            //assembler.ret();

            //// Encode the instructions
            //assembler.Assemble(codeWriter, 0);
            //var bytes = codeWriter.ToArray();
            // return bytes;
        }

        /// <summary>
        /// Orignial Instructions (Usually function prologue)
        /// Stack detour to original func + (Orignial Instructions Length).
        /// </summary>
        /// <param name="migratedCodes"></param>
        /// <param name="offsetStackAddr"></param>
        /// <returns></returns>
        public byte[] BuildTrampoline64(byte[] migratedCodes, nint offsetStackAddr) {
            var finalDetour = BuildStackDetour64(offsetStackAddr);
            var trampolineCodes = new byte[migratedCodes.Length + finalDetour.Length];

            migratedCodes.CopyTo(trampolineCodes, 0);
            finalDetour.CopyTo(trampolineCodes, migratedCodes.Length);
            return trampolineCodes;
        }
    }
}
