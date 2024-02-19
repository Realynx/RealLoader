namespace PalworldManagedModFramework.Sdk.Attributes {
    public class HookAttribute : Attribute {
        public HookAttribute(string pattern, bool execute, bool overrideReturn, bool branchThread = false) {
            Pattern = pattern;
            Execute = execute;
            OverrideReturn = overrideReturn;
            BranchThread = branchThread;
        }

        public string Pattern { get; }
        public bool Execute { get; }
        public bool OverrideReturn { get; }
        public bool BranchThread { get; }
    }
}
