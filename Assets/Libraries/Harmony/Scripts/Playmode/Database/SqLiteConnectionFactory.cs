using System;
using System.Data.Common;
using Mono.Data.Sqlite;
using System.IO;
using UnityEngine;

namespace Harmony
{
    /// <summary>
    /// SqLite DbConnection factory.
    /// </summary>
    [Findable(R.S.Tag.MainController)]
    public class SqLiteConnectionFactory : MonoBehaviour
    {
        //#Dirty Fix : Unity player (not Editor) does not load DLLs that depends on other DLLs
#if !UNITY_EDITOR
        static void SqLiteRepository()
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
        
        private const string SQLITE_CONNECTION_TEMPLATE = "URI=file:{0}";

        [SerializeField] private string databaseFileName = "Database.db";

        private string connexionString;

        private void Awake()
        {
            connexionString = GetConnexionString();
        }
        
        /// <summary>
        /// Gets a new DbConnection for the configured database.
        /// </summary>
        /// <returns>New DBConnection.</returns>
        public DbConnection GetConnection()
        {
            CreateDatabaseIfDoesntExits();
            
            return new SqliteConnection(connexionString);
        }

        /// <summary>
        /// Path to the database file in the user home folder.
        /// </summary>
        /// <returns>Path.</returns>
        public string GetCurrentDatabaseFilePath()
        {
            return Path.Combine(ApplicationExtensions.PersistentDataPath, databaseFileName);
        }

        /// <summary>
        /// Path to the database file in the project assets.
        /// </summary>
        /// <returns>Path.</returns>
        public string GetSourceDatabaseFilePath()
        {
            return Path.Combine(ApplicationExtensions.ApplicationDataPath, databaseFileName);
        }

        /// <summary>
        /// Copies the database file in the projects assets to the user home folder if it does not already exits.
        /// </summary>
        public void CreateDatabaseIfDoesntExits()
        {
            if (IsCurrentDatabaseDoesntExists())
            {
                File.Copy(GetSourceDatabaseFilePath(), GetCurrentDatabaseFilePath(), true);
            }
        }

        /// <summary>
        /// Copies the database file in the projects assets to the user home folder.
        /// </summary>
        public void ResetDatabase()
        {
            if (IsCurrentDatabaseExists())
            {
                File.Delete(GetCurrentDatabaseFilePath());
            }
        }

        /// <summary>
        /// Checks if the database exists in the user home folder.
        /// </summary>
        /// <returns>True if it exits, false otherwise.</returns>
        public bool IsCurrentDatabaseExists()
        {
            return File.Exists(GetCurrentDatabaseFilePath());
        }

        /// <summary>
        /// Checks if the database doesn't exists in the user home folder.
        /// </summary>
        /// <returns>True if it doesn't exits, false otherwise.</returns>
        public bool IsCurrentDatabaseDoesntExists()
        {
            return !IsCurrentDatabaseExists();
        }

        /// <summary>
        /// Checks if the database exists in the project assets.
        /// </summary>
        /// <returns>True if it exits, false otherwise.</returns>
        public bool IsSourceDatabaseExists()
        {
            return File.Exists(GetSourceDatabaseFilePath());
        }

        /// <summary>
        /// Checks if the database doesn't exists in the project assets.
        /// </summary>
        /// <returns>True if it doesn't exits, false otherwise.</returns>
        public bool IsSourceDatabaseDoesntExists()
        {
            return !IsSourceDatabaseExists();
        }

        private string GetConnexionString()
        {
            return String.Format(SQLITE_CONNECTION_TEMPLATE, GetCurrentDatabaseFilePath());
        }
    }
}
