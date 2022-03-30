using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SoMeSimulator.Data.Models.Defaults;

namespace SoMeSimulator.Helpers
{
    public static class AlphaNumericCode
    {
        private static readonly Random Random = new Random();

        /// <summary>
        /// Generates a code with length
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Generate(int length = 5)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

    }
}
