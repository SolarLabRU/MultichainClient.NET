namespace Platform.DataAccess.MultiChain.Client
{
    public enum CheckBlockType
    {
        ReadFromDisk = 0,
        EnsureEachBlockIsValid = 1,
        CheckCanReadUndoFiles = 2,
        TestEachBlockUndo = 3,
        ReconnectUndoneBlocks = 4
    }
}
