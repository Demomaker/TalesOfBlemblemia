using UnityEngine;

namespace Harmony
{
    /// <summary>
    /// Extension methods for floats.
    /// </summary>
    public static class FloatExtensions
    {
        /// <summary>
        /// Rounds this float to it closest int value.
        /// </summary>
        public static int RoundToInt(this float value)
        {
            return Mathf.RoundToInt(value);
        }
    }
}