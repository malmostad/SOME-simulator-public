using System;
using System.Collections.Generic;
using System.Linq;
using SoMeSimulator.Data.Models;

namespace SoMeSimulator.Helpers
{
    static class Extensions
    {

        /// <summary>
        /// Selects random item from collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static T PickRandom<T>(this IEnumerable<T> enumerable)
        {
            var list = enumerable.ToList();
            var toSkip = new Random().Next(0, list.ToList().Count);
            return list.Skip(toSkip).Take(1).SingleOrDefault();
        }

        /// <summary>
        /// Find Entity by I
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static T FindById<T>(this IEnumerable<T> enumerable, int id) where T : EntityBase
        {
            return enumerable.SingleOrDefault(e => e.Id == id);
        }
    }
}
