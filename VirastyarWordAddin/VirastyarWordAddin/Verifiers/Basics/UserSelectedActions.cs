using System;
namespace VirastyarWordAddin.Verifiers.Basics
{
    [Flags]
    public enum UserSelectedActions
    {
        Change = 0x1, 
        ChangeAll = 0x2, 
        Ignore = 0x4, 
        IgnoreAll = 0x8, 
        Stop = 0x10, 
        AddToDictionary = 0x20,
        Resume = 0x40,
        None = 0x80
    }
}
