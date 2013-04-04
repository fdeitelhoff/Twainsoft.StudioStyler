// Guids.cs
// MUST match guids.h

using System;

namespace Twainsoft.StudioStyler.VSPackage
{
    static class GuidList
    {
        public const string guidTwainsoft_StudioStyler_VSPackagePkgString = "eb3bc074-f91c-4654-b850-df23e2082845";
        public const string guidTwainsoft_StudioStyler_VSPackageCmdSetString = "8c817fba-98c3-459f-b94c-ef299123e600";
        public const string guidToolWindowPersistanceString = "e5933aa6-2e6c-440c-aec4-bf822bf1a4c8";

        public static readonly Guid guidTwainsoft_StudioStyler_VSPackageCmdSet = new Guid(guidTwainsoft_StudioStyler_VSPackageCmdSetString);
    };
}