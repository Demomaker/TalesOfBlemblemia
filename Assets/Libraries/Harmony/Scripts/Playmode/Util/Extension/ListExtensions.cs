using System;
using System.Collections.Generic;

namespace Harmony
{
    /// <summary>
    /// Extension methods for Lists.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Return random element in the list.
        /// </summary>
        /// <param name="list">List.</param>
        /// <returns>Random element in the list.</returns>
        public static T Random<T>(this List<T> list)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        /// <summary>
        /// Return random element in the list.
        /// </summary>
        /// <param name="list">List.</param>
        /// <param name="random">Random provider.</param>
        /// <returns>Random element in the list.</returns>
        public static T Random<T>(this List<T> list, Random random)
        {
            return list[random.Next(0, list.Count)];
        }

        /// <summary>
        /// Remove random element in the list.
        /// </summary>
        /// <param name="list">List.</param>
        /// <returns>Removed element.</returns>
        public static T RemoveRandom<T>(this List<T> list)
        {
            var randomIndex = UnityEngine.Random.Range(0, list.Count);
            var randomValue = list[randomIndex];
            list.RemoveAt(randomIndex);
            return randomValue;
        }

        /// <summary>
        /// Remove random element in the list.
        /// </summary>
        /// <param name="list">List.</param>
        /// <param name="random">Random provider.</param>
        /// <returns>Removed element.</returns>
        public static T RemoveRandom<T>(this List<T> list, Random random)
        {
            var randomIndex = random.Next(0, list.Count);
            var randomValue = list[randomIndex];
            list.RemoveAt(randomIndex);
            return randomValue;
        }
    }
}