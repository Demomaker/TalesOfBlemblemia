using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

namespace Harmony
{
    /// <summary>
    /// Abstrract repository implementation.
    /// </summary>
    /// <typeparam name="T">Type contained in the database.</typeparam>
    public abstract class SqLiteRepository<T> : MonoBehaviour where T : class
    {
        //#Dirty Fix : Unity player (not Editor) does not load DLLs that depends on other DLLs
#if !UNITY_EDITOR
        static SqLiteRepository()
        {
            string path = System.Environment.GetEnvironmentVariable("PATH", System.EnvironmentVariableTarget.Process);
            string pluginPath = UnityEngine.Application.dataPath + System.IO.Path.DirectorySeparatorChar + "Plugins";
            if (path.Contains(pluginPath) == false)
            {
                System.Environment.SetEnvironmentVariable("PATH", path + System.IO.Path.PathSeparator + pluginPath,
                                                          System.EnvironmentVariableTarget.Process);
            }
        }

#endif

        private readonly SqLiteConnectionFactory connectionFactory;
        private readonly SqLiteParameterFactory parameterFactory;
        private readonly SqLiteDataMapper<T> dataMapper;

        /// <summary>
        /// Creates new SqLiteRepository.
        /// </summary>
        /// <param name="connectionFactory">
        /// DbConnection factory. To create database connections for each query.
        /// </param>
        /// <param name="parameterFactory">
        /// DBParameter factory. To create parameters for each query.
        /// </param>
        /// <param name="dataMapper">
        /// DataMapper. To convert database rows into objects.
        /// </param>
        protected SqLiteRepository(SqLiteConnectionFactory connectionFactory,
                                   SqLiteParameterFactory parameterFactory,
                                   SqLiteDataMapper<T> dataMapper)
        {
            this.connectionFactory = connectionFactory;
            this.parameterFactory = parameterFactory;
            this.dataMapper = dataMapper;
        }

        /// <summary>
        /// Query the database to get only one value. Useful for "Count" or "Sum" queries.
        /// </summary>
        /// <param name="commandSql">
        /// SQL query. Must produce only one row and one column.
        /// </param>
        /// <param name="parametersValues">
        /// Parameters for the query. Only base types are supported.
        /// </param>
        /// <returns>Read scalar.</returns>
        protected long ExecuteScalar(string commandSql, object[] parametersValues)
        {
            return ExecuteQueryScallar(commandSql, parametersValues);
        }

        /// <summary>
        /// Query the database to get only one object.
        /// </summary>
        /// <param name="commandSql">
        /// SQL query. Only one row will be read. Any other row will be ignored. 
        /// </param>
        /// <param name="parametersValues">
        /// Parameters for the query. Only base types are supported.
        /// </param>
        /// <returns>Read object.</returns>
        protected T ExecuteSelectOne(string commandSql, object[] parametersValues)
        {
            return ExecuteQueryOne(commandSql, parametersValues);
        }

        /// <summary>
        /// Query the database to get all objects.
        /// </summary>
        /// <param name="commandSql">
        /// SQL query.
        /// </param>
        /// <param name="parametersValues">
        /// Parameters for the query. Only base types are supported.
        /// </param>
        /// <returns>List of read objects. The list is never null, although it can be empty.</returns>
        protected IList<T> ExecuteSelectAll(string commandSql, object[] parametersValues)
        {
            return ExecuteQueryList(commandSql, parametersValues);
        }

        /// <summary>
        /// Write a new row into the database.
        /// </summary>
        /// <param name="commandSql">
        /// SQL query.
        /// </param>
        /// <param name="parametersValues">
        /// Parameters for the query. Only base types are supported.
        /// </param>
        /// <returns>Last inserted row id.</returns>
        protected long ExecuteInsert(string commandSql, object[] parametersValues)
        {
            return ExecuteNonQueryWithId(commandSql, parametersValues);
        }

        /// <summary>
        /// Update rows on the database.
        /// </summary>
        /// <param name="commandSql">
        /// SQL query.
        /// </param>
        /// <param name="parametersValues">
        /// Parameters for the query. Only base types are supported.
        /// </param>
        protected void ExecuteUpdate(string commandSql, object[] parametersValues)
        {
            ExecuteNonQuery(commandSql, parametersValues);
        }

        /// <summary>
        /// Delete rows on the database.
        /// </summary>
        /// <param name="commandSql">
        /// SQL query. 
        /// </param>
        /// <param name="parametersValues">
        /// Parameters for the query. Only base types are supported.
        /// </param>
        protected void ExecuteDelete(string commandSql, object[] parametersValues)
        {
            ExecuteNonQuery(commandSql, parametersValues);
        }

        private long ExecuteQueryScallar(string commandSql, object[] parametersValues)
        {
            using (var connection = connectionFactory.GetConnection())
            {
                connection.Open();

                using (var dbCommand = GetCommand(connection, commandSql, parametersValues))
                {
                    return (long) dbCommand.ExecuteScalar();
                }
            }
        }

        private T ExecuteQueryOne(string commandSql, object[] parametersValues)
        {
            using (var connection = connectionFactory.GetConnection())
            {
                connection.Open();

                using (var command = GetCommand(connection, commandSql, parametersValues))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return dataMapper.GetObjectFromReader(reader);
                        }
                    }
                }
            }

            return null;
        }

        private IList<T> ExecuteQueryList(string commandSql, object[] parametersValues)
        {
            IList<T> dataObjects = new List<T>();
            using (var connection = connectionFactory.GetConnection())
            {
                connection.Open();

                using (var command = GetCommand(connection, commandSql, parametersValues))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dataObjects.Add(dataMapper.GetObjectFromReader(reader));
                        }
                    }
                }
            }

            return dataObjects;
        }

        private void ExecuteNonQuery(string commandSql, object[] parametersValues)
        {
            using (var connection = connectionFactory.GetConnection())
            {
                connection.Open();

                using (var command = GetCommand(connection, commandSql, parametersValues))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private long ExecuteNonQueryWithId(string commandSql, object[] parametersValues)
        {
            using (var connection = connectionFactory.GetConnection())
            {
                connection.Open();

                using (var command = GetCommand(connection, commandSql, parametersValues))
                {
                    command.ExecuteNonQuery();

                    return dataMapper.GetPrimaryKeyFromConnection(connection);
                }
            }
        }

        private DbCommand GetCommand(DbConnection connection, string commandSql, object[] parametersValues)
        {
            var command = connection.CreateCommand();
            command.CommandText = commandSql;
            foreach (var value in parametersValues)
            {
                command.Parameters.Add(parameterFactory.GetParameter(value));
            }

            return command;
        }
    }
}