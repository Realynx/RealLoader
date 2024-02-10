namespace PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.FLags {
    public enum ELifetimeCondition : byte {
        COND_None = 0x0,
        COND_InitialOnly = 0x1,
        COND_OwnerOnly = 0x2,
        COND_SkipOwner = 0x3,
        COND_SimulatedOnly = 0x4,
        COND_AutonomousOnly = 0x5,
        COND_SimulatedOrPhysics = 0x6,
        COND_InitialOrOwner = 0x7,
        COND_Custom = 0x8,
        COND_ReplayOrOwner = 0x9,
        COND_ReplayOnly = 0xA,
        COND_SimulatedOnlyNoReplay = 0xB,
        COND_SimulatedOrPhysicsNoReplay = 0xC,
        COND_SkipReplay = 0xD,
        COND_Never = 0xF,
        COND_NetGroup = 0x10,
        COND_Max = 0x11,
    }
}
