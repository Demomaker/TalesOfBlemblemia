using UnityEditor;
using UnityEngine;

namespace Harmony
{
    /// <summary>
    /// Extension methods for the Application.
    /// </summary>
    public static class ApplicationExtensions
    {
        /// <summary>
        /// Path to the data folder of the application. This folder is read only.
        /// </summary>
        public static string ApplicationDataPath => Application.streamingAssetsPath;

        /// <summary>
        /// Path to the persistent storage folder of the application. This folder is read/write.
        /// </summary>
        public static string PersistentDataPath => Application.persistentDataPath;

        /// <summary>
        /// Closes the application.
        /// </summary>
        public static void Quit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}