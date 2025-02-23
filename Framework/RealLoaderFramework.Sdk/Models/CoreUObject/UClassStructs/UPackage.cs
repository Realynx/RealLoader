﻿using System.Runtime.InteropServices;

using RealLoaderFramework.Sdk.Models.CoreUObject.GNameStructs;

namespace RealLoaderFramework.Sdk.Models.CoreUObject.UClassStructs {
    /// <summary>
    /// <see href="https://docs.unrealengine.com/5.3/en-US/API/Runtime/CoreUObject/UObject/UPackage/"/>
    /// |
    /// <see href="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Public/UObject/Package.h#L134C1-L135C1"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct UPackage {
        public FNameEntryId Name {
            get {
                return baseUObject.baseObjectBaseUtility.baseUObjectBase.namePrivate.comparisonIndex;
            }
        }
        //Inherit
        public UObject baseUObject;

        public bool bDirty;
        public bool bHasBeenFullyLoaded;
        public bool bCanBeImported;

        private byte _alignment_a;
        private byte _alignment_b;
        private byte _alignment_c;

        public FGuid guid;
        public uint packageFlagsPrivate;
        public FPackageId packageId;
        public FPackagePath LoadedPath;
        public FPackageFileVersion LinkerPackageVersion;
        public int LinkerLicenseeVersion;
        public FCustomVersionContainer LinkerCustomVersion;

        public FLinkerLoad* LinkerLoad;

        public ulong FileSize;
        public FName FileName;

        public void* WorldTileInfo; // TUniquePtr<FWorldTileInfo, TDefaultDelete<FWorldTileInfo>_>
    }
}
