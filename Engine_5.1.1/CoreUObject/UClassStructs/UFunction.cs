using System.Runtime.InteropServices;

using Engine_5._1._1.CoreUObject.Flags;

namespace Engine_5._1._1.CoreUObject.UClassStructs {
    /// <summary>
    /// <see href="https://docs.unrealengine.com/5.1/en-US/API/Runtime/CoreUObject/UObject/UFunction/"/>
    /// |
    /// <see href="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Public/UObject/Class.h#L1714"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct UFunction {
        // inherits
        public UStruct baseUstruct;

        // Public

        /// <summary>
        /// EFunctionFlags set defined for this function.
        /// </summary>
        public EFunctionFlags functionFlags;

        /// <summary>
        /// Number of parameters total.
        /// </summary>
        public byte numParams;

        /// <summary>
        /// Total size of parameters in memory.
        /// </summary>
        public ushort paramSize;

        /// <summary>
        /// Memory offset of return value property.
        /// </summary>
        public ushort returnValueOffset;

        /// <summary>
        /// Id of this RPC function call (must be FUNC_Net & (FUNC_NetService|FUNC_NetResponse)).
        /// </summary>
        public short rpcId;

        /// <summary>
        /// Id of the corresponding response call (must be FUNC_Net & FUNC_NetService).
        /// </summary>
        public short rpcResponseId;

        /// <summary>
        /// pointer to first local struct property in this UFunction that contains defaults.
        /// </summary>
        public FProperty* firstPropertyToInit;
    }
}
