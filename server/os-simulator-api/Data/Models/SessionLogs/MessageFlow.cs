using System;

namespace SoMeSimulator.Data.Models.SessionLogs
{
    [Flags] 
    public enum MessageFlow: uint
    {
        Short = 1,
        Long = 2
    }
}