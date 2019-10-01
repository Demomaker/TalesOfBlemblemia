using UnityEditor;
using UnityEngine;

namespace Harmony
{
    public abstract class BasePropertyDrawer<T> : PropertyDrawer where T : class
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            Draw(attribute as T, property, position);
            EditorGUI.EndProperty();
        }

        protected abstract void Draw(T attribute, SerializedProperty property, Rect position);
    }
}