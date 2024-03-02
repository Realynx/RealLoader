using System.Text;

using ClangSharp.Interop;

namespace UnrealCoreObjectApiSourceGen.Services.SourceGen.Converters.CSharpConverter {
    public class FieldConverter : IFieldConverter {
        public FieldConverter() {

        }

        public void ConvertFieldNode(CXCursor cursor, CXCursor parent, StringBuilder codeBuilder) {
            var attribute = GenerateOffsetAttribute(cursor, parent, codeBuilder);
        }

        private string GenerateOffsetAttribute(CXCursor cursor, CXCursor parent, StringBuilder codeBuilder) {
            // Console.WriteLine($"0x{cursor.Type.AddressSpace:X}");
            //var offset = cursor.Type.SizeOf;

            var offset = GetFieldOffset(cursor, parent);

            var attributeComponent = $"    [FieldOffset(0x{offset:X})]";
            codeBuilder.AppendLine(attributeComponent);

            var fieldComponent = $"    public {cursor.Type.Spelling} {cursor.DisplayName};";
            codeBuilder.AppendLine(fieldComponent);

            codeBuilder.AppendLine();

            //var attributeComponent = $"    [FieldOffset(0x{offset:X})]";
            //codeBulder.AppendLine(attributeComponent);

            //var fieldComponent = $"    public {cursor.Type.Spelling} {cursor.DisplayName};";
            //codeBulder.AppendLine(fieldComponent);

            return codeBuilder.ToString();
        }

        private static long GetFieldOffset(CXCursor cursor, CXCursor parent) {
            var offset = 0l;

            var baseSize = parent.Type.BaseType.SizeOf;
            if (baseSize != long.MaxValue) {
                offset = baseSize;
            }

            var currentfieldIndex = cursor.FieldIndex;

            for (var x = 0u; x < parent.NumFields; x++) {
                var field = parent.GetField(x);
                if (currentfieldIndex == x) {
                    break;
                }

                offset += field.Type.SizeOf;
            }

            return offset;
        }
    }
}
