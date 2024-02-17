namespace PalworldManagedModFramework.Sdk.Attributes {
    public class HookAttribute : Attribute {
        public HookAttribute(string pattern, bool execute, bool overideReturn, bool branchThread = false) {
            Pattern = pattern;
            Execute = execute;
            OverideReturn = overideReturn;
            BranchThread = branchThread;
        }

        public string Pattern { get; }
        public bool Execute { get; }
        public bool OverideReturn { get; }
        public bool BranchThread { get; }
    }
}
