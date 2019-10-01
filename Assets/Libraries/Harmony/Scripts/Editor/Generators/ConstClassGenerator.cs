using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Harmony
{
    public static class ConstClassGenerator
    {
        [MenuItem("Tools/Harmony/Generate Const Classes", priority = 100)]
        public static void GenerateConstClasses()
        {
            GenerateScripts();
            ReloadGeneratedScripts();
        }

        private static void GenerateScripts()
        {
            var pathToScript = Path.GetFullPath(Path.Combine(Application.dataPath, "../GenerateCode.bat"));
            var pathToProjectDirectory = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            var pathToGeneratedDirectory = Path.GetFullPath(Path.Combine(Application.dataPath, "Generated"));

            if (pathToScript.Contains(" ") || pathToProjectDirectory.Contains(" ") || pathToGeneratedDirectory.Contains(" "))
            {
                Debug.LogError("You cannot use the Code Generator this project : his path must not have spaces in it. " +
                               "Stop Unity, move your project, and try again.");
            }
            else
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "cmd",
                    Arguments = "/c \"" + pathToScript + "\"",
                };
                Process.Start(processStartInfo);
            }
        }

        private static void ReloadGeneratedScripts()
        {
            foreach (var file in Directory.GetFiles(Path.Combine("Assets", "Generated"), "*.cs", SearchOption.AllDirectories))
                AssetDatabase.ImportAsset(file);
        }
    }
}