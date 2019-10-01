using System;
using System.Collections.Generic;

namespace Harmony
{
    /// <summary>
    /// Reusable list. This list is cleared after each use.
    /// </summary>
    /// <typeparam name="T">Content type.</typeparam>
    public class ReusableList<T>
    {
        private List<T> list;

        /// <summary>
        /// Use the list while in the provided function. 
        /// </summary>
        /// <param name="useFunction">Function that uses the list.</param>
        /// <returns>Function return value.</returns>
        public TReturn Use<TReturn>(Func<List<T>, TReturn> useFunction)
        {
            try
            {
                return useFunction(GetList());
            }
            finally
            {
                ClearList();
            }
        }

        private List<T> GetList()
        {
            return list ?? (list = new List<T>());
        }

        private void ClearList()
        {
            list?.Clear();
        }
    }
}