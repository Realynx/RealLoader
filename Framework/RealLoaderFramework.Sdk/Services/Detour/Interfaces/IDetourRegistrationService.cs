﻿namespace RealLoaderFramework.Sdk.Services.Detour.Interfaces {
    public interface IDetourRegistrationService {
        IDetourRegistrationService FindAndRegisterDetours<TType>();
    }
}