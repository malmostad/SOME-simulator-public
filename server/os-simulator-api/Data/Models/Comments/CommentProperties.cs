using System;

namespace SoMeSimulator.Data.Models.Comments
{
    [Flags]
    public enum CommentProperties: uint
    {
        Positive = 1,
        Negative = 2,
        Neutral = 4,

        Easy = 8,
        Difficult = 16,

        Reply = 32,
    }
}