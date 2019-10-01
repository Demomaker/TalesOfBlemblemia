using System.Data.Common;
using Mono.Data.Sqlite;
using UnityEngine;

namespace Harmony
{
    /// <summary>
    /// DbParameter factory. Converts objects into DbParameter.
    /// </summary>
    public class SqLiteParameterFactory : MonoBehaviour
    {
        /// <summary>
        /// Convert provided object into a DbParameter.
        /// </summary>
        /// <param name="value">
        /// Object to convert. Can be null.
        /// </param>
        /// <returns>Converted object.</returns>
        public DbParameter GetParameter(object value)
        {
            return new SqliteParameter {Value = value};
        }
    }
}