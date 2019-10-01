using System.Data.Common;

namespace Harmony
{
    /// <summary>
    /// Data mapper. Converts database rows to objects.
    /// </summary>
    public abstract class SqLiteDataMapper<T> where T : class
    {
        /// <summary>
        /// Converts provided database row to an object.
        /// </summary>
        /// <param name="reader">Row to convert.</param>
        /// <returns>Converted row.</returns>
        public abstract T GetObjectFromReader(DbDataReader reader);

        /// <summary>
        /// Get last inserted row id.
        /// </summary>
        /// <param name="connection">DbConncetion where the insertion occured.</param>
        /// <returns>Last generated row id.</returns>
        public long GetPrimaryKeyFromConnection(DbConnection connection)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT last_insert_rowid()";
            return (long) command.ExecuteScalar();
        }
    }
}