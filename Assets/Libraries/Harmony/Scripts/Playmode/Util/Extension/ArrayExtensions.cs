using System;

namespace Harmony
{
    /// <summary>
    /// Extension methods for Arrays.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Fill an array with the provided value.
        /// </summary>
        /// <param name="array">Array.</param>
        /// <param name="value">Value to use.</param>
        /// <returns>Filled array. This is the same array, not a copy of the array.</returns>
        public static T[] Fill<T>(this T[] array, T value)
        {
            for (var i = 0; i < array.Length; i++) array[i] = value;
            return array;
        }

        /// <summary>
        /// Fill a 2D array with the provided value.
        /// </summary>
        /// <param name="array">Array.</param>
        /// <param name="value">Value to use.</param>
        /// <returns>Filled array. This is the same array, not a copy of the array.</returns>
        public static T[,] Fill<T>(this T[,] array, T value)
        {
            for (var i = 0; i < array.GetLength(0); i++)
            for (var j = 0; j < array.GetLength(1); j++)
                array[i, j] = value;
            return array;
        }

        /// <summary>
        /// Return random element in array.
        /// </summary>
        /// <param name="array">Array.</param>
        /// <returns>Random element in array.</returns>
        public static T Random<T>(this T[] array)
        {
            return array[UnityEngine.Random.Range(0, array.Length)];
        }
        
        /// <summary>
        /// Return random element in array.
        /// </summary>
        /// <param name="array">Array.</param>
        /// <param name="random">Random provider.</param>
        /// <returns>Random element in array.</returns>
        public static T Random<T>(this T[] array, Random random)
        {
            return array[random.Next(0, array.Length)];
        }
    }
}