using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Harmony
{
    [CustomEditor(typeof(SceneBundle), true)]
    public class SceneBundleInspector : BaseInspector
    {
        private SceneBundle sceneBundle;

        protected override void Initialize()
        {
            sceneBundle = target as SceneBundle;
        }

        protected override void Draw()
        {
            DrawDefault();
            DrawInformations();
            DrawTools();
        }

        private void DrawInformations()
        {
            if (EditorBuildSettings.scenes.Length == 0)
                DrawWarningBox("Build settings are empty. None of theses scenes can be loaded.");

            DrawTitleLabel("Scenes");

            var scenesNotInTheBuildSettings = sceneBundle.Scenes
                .Where(it =>
                    it.SceneAsset != null && EditorBuildSettings.scenes.All(other =>
                        Path.GetFileNameWithoutExtension(other.path) != it.Name))
                .ToList();
            if (scenesNotInTheBuildSettings.Count > 0)
                DrawErrorBox(
                    "Theses scenes are not in the Build Settings, but are used in this bundle. \n\n" +
                    scenesNotInTheBuildSettings.Select(it => it.Name)
                        .Aggregate((current, next) => current + "\n" + next));
        }

        private void DrawTools()
        {
            BeginTable();
            BeginTableCol();
            DrawTitleLabel("Scene Bundle Tools");

            if (!EditorApplication.isPlaying && EditorBuildSettings.scenes.Length > 0)
            {
                DrawWarningBox("This will also load the first scene in the BuildSettings.");
                DrawButton("Open in editor", OpenInEditor);
            }
            else
            {
                DrawDisabledButton("Open in editor");
            }

            EndTableCol();
            EndTable();
        }

        private void OpenInEditor()
        {
            if (EditorBuildSettings.scenes.Any())
            {
                //Load Main scene.
                SceneManager.SetActiveScene(
                    EditorSceneManager.OpenScene(EditorBuildSettings.scenes[0].path, OpenSceneMode.Single)
                );
            }

            //Load bundle scenes
            foreach (var scene in sceneBundle.Scenes)
                EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(scene.SceneAsset), OpenSceneMode.Additive);
        }
    }
}