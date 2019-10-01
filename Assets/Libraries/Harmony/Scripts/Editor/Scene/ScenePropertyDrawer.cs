using System.IO;
using UnityEditor;
using UnityEngine;

namespace Harmony
{
    [CustomPropertyDrawer(typeof(SceneReference))]
    public class SceneReferencePropertyDrawer : BasePropertyDrawer<SceneReference>
    {
        protected override void Draw(SceneReference attribute, SerializedProperty property, Rect position)
        {
            EditorGUI.BeginProperty(position, GUIContent.none, property);

            var sceneAsset = property.FindPropertyRelative("sceneAsset");
            var sceneName = property.FindPropertyRelative("sceneName");

            if (sceneAsset != null)
            {
                EditorGUI.BeginChangeCheck();
                var value = EditorGUI.ObjectField(position, sceneAsset.objectReferenceValue, typeof(SceneAsset), false);
                if (EditorGUI.EndChangeCheck())
                {
                    sceneAsset.objectReferenceValue = value;
                    if (sceneAsset.objectReferenceValue != null)
                    {
                        var scenePath = AssetDatabase.GetAssetPath(sceneAsset.objectReferenceValue);
                        sceneName.stringValue = Path.GetFileNameWithoutExtension(scenePath);
                    }
                }
            }

            EditorGUI.EndProperty();
        }
    }
}