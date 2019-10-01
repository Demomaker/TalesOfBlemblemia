using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Harmony
{
    [CustomEditor(typeof(SqLiteConnectionFactory), true)]
    public class SqLiteConnectionFactoryInspector : BaseInspector
    {
        private SqLiteConnectionFactory sqLiteConnectionFactory;

        protected override void Initialize()
        {
            sqLiteConnectionFactory = target as SqLiteConnectionFactory;
        }

        protected override void Draw()
        {
            Initialize();
            DrawDefault();

            if (sqLiteConnectionFactory.IsSourceDatabaseExists())
            {
                sqLiteConnectionFactory.CreateDatabaseIfDoesntExits();

                BeginTable();
                BeginTableCol();
                DrawTitleLabel("Database utils");
                DrawLabel("Open the source database file of the project.");
                DrawButton("Open Source Database", OpenSourceDatabase);
                DrawLabel("Open the current database file of the user.");
                DrawButton("Open Current Database", OpenCurrentDatabase);
                DrawLabel("Reset the current database file of the user.");
                DrawButton("Reset Current Database", ResetCurrentDatabase);
                EndTableCol();
                EndTable();
            }
            else
            {
                DrawErrorBox("Database doesn't exists. Make sure there's a database in the \"StreamingAssets\" " +
                             "folder. You can also create it.");
                DrawButton("Create Source Database", CreateSourceDatabase);
            }
        }

        private void OpenSourceDatabase()
        {
            OpenSourceDatabase(sqLiteConnectionFactory.GetSourceDatabaseFilePath());
        }

        private void OpenCurrentDatabase()
        {
            sqLiteConnectionFactory.CreateDatabaseIfDoesntExits();

            OpenSourceDatabase(sqLiteConnectionFactory.GetCurrentDatabaseFilePath());
        }

        private void ResetCurrentDatabase()
        {
            sqLiteConnectionFactory.ResetDatabase();
        }

        private void CreateSourceDatabase()
        {
            CreateSourceDatabase(sqLiteConnectionFactory.GetSourceDatabaseFilePath());
        }

        private void OpenSourceDatabase(string databasePath)
        {
            var pathToDbBrowser = Path.GetFullPath(Path.Combine(Application.dataPath, "../Tools/SQLiteDBBrowser/SQLiteDBBrowser.exe"));
            var processStartInfo = new ProcessStartInfo {FileName = pathToDbBrowser, Arguments = "\"" + databasePath + "\""};
            Process.Start(processStartInfo);
        }

        private void CreateSourceDatabase(string databasePath)
        {
            var pathToEmpty = Path.GetFullPath(Path.Combine(Application.dataPath, "../Tools/SQLiteDBBrowser/Empty.db"));
            var pathToNew = Path.GetFullPath(databasePath);

            var streamingAssetsFolder = Path.GetFullPath(Application.streamingAssetsPath);
            if (!Directory.Exists(streamingAssetsFolder)) Directory.CreateDirectory(streamingAssetsFolder);
            File.Copy(pathToEmpty, pathToNew);

            AssetDatabase.Refresh();
        }
    }
}