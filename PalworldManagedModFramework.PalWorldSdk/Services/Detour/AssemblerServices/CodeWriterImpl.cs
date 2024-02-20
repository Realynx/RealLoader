using Iced.Intel;

namespace PalworldManagedModFramework.Sdk.Services.Detour.AssemblerServices {

    class CodeWriterImpl : CodeWriter {
        private readonly List<byte> _bytes = new List<byte>();

        public override void WriteByte(byte value) {
            _bytes.Add(value);
        }

        public byte[] ToArray() {
            return _bytes.ToArray();
        }
    }
}
