using System;

namespace VirastyarWordAddin
{
    [Flags]
    public enum VerificationWindowButtons
    {
        None = 0,
        Change = 1,
        ChangeAll = 2,
        Ignore = 4,
        IgnoreAll = 8,
        AddToDictionary = 16,
        Stop = 32
    }
}
