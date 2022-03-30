using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoMeSimulator.Data.Models.SessionLogs
{
    [Flags]
    public enum BotReplyProperties: uint
    {
        Neutral = 1,
        Positive = 2,
        Negative = 4,
    }
}