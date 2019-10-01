namespace Harmony
{
    /// <summary>
    /// Extension methods for Strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Replaces the format item in a specified string with the string representation of a
        /// corresponding object in a specified array.
        /// </summary>
        /// <param name="value">Format string.</param>
        /// <param name="objects">Objects.</param>
        /// <returns>Formatted string.</returns>
        public static string Format(this string value, params object[] objects)
        {
            return string.Format(value, objects);
        }
    }
}