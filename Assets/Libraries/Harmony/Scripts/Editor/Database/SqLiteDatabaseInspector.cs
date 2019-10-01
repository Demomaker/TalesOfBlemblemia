using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Harmony
{
    [CustomEditor(typeof(DefaultAsset))]
    public class SqLiteDatabaseInspector : BaseInspector
    {
        private string filePath;

        protected override void Initialize()
        {
            filePath = AssetDatabase.GetAssetPath(target);
        }

        protected override void Draw()
        {
            Initialize();

            if (filePath.EndsWith(".db"))
            {
                GUI.enabled = true; //File editor have everything disabled by default. #Dirty fix
                DrawButton("Open Database in DbBrowser", OpenDatabase);
                GUI.enabled = false;
            }
            else
            {
                DrawDefault();
            }
        }

        private void OpenDatabase()
        {
            var pathToDbBrowser =
                Path.GetFullPath(Path.Combine(Application.dataPath, "../Tools/SQLiteDBBrowser/SQLiteDBBrowser.exe"));
            var processStartInfo = new ProcessStartInfo
            {
                FileName = pathToDbBrowser,
                Arguments = "\"" + Application.dataPath + "/../" + filePath + "\""
            };
            Process.Start(processStartInfo);
        }
    }
}