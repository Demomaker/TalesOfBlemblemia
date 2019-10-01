using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Harmony
{
    public abstract class BaseInspector : Editor
    {
        private bool isRefreshing;

        private void Awake()
        {
            Initialize();
        }

        protected void DrawDefault()
        {
            DrawDefaultInspector();
        }

        protected BasicProperty GetBasicProperty(string name)
        {
            return new BasicProperty(serializedObject.FindProperty(name));
        }
        
        protected GridProperty GetGridProperty(string name, int nbCols = 3)
        {
            return new GridProperty(serializedObject.FindProperty(name), nbCols);
        }

        protected void DrawProperty(ICustomProperty property)
        {
            if (isRefreshing) return;

            //"property" might be null. Thus, we can't convert to method group.
            //ReSharper disable once ConvertClosureToMethodGroup
            RefreshIfNeededOrDraw(property, () => property.Draw());
        }

        protected void DrawPropertyWithLabel(ICustomProperty property)
        {
            if (isRefreshing) return;

            //"property" might be null. Thus, we can't convert to method group.
            //ReSharper disable once ConvertClosureToMethodGroup
            RefreshIfNeededOrDraw(property, () => property.DrawWithLabel());
        }


        protected void DrawPropertyWithTitleLabel(ICustomProperty property)
        {
            if (isRefreshing) return;

            //"property" might be null. Thus, we can't convert to method group.
            //ReSharper disable once ConvertClosureToMethodGroup
            RefreshIfNeededOrDraw(property, () => property.DrawWithTitleLabel());
        }

        protected void BeginHorizontal()
        {
            if (isRefreshing) return;

            EditorGUILayout.BeginHorizontal();
        }

        protected void EndHorizontal()
        {
            if (isRefreshing) return;

            EditorGUILayout.EndHorizontal();
        }

        protected void BeginVertical()
        {
            if (isRefreshing) return;

            EditorGUILayout.BeginVertical();
        }

        protected void EndVertical()
        {
            if (isRefreshing) return;

            EditorGUILayout.EndVertical();
        }

        protected void BeginTable()
        {
            if (isRefreshing) return;

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        }

        protected void EndTable()
        {
            if (isRefreshing) return;

            EditorGUILayout.EndHorizontal();
        }

        protected void BeginTableRow()
        {
            if (isRefreshing) return;

            EditorGUILayout.BeginHorizontal();
        }

        protected void EndTableRow()
        {
            if (isRefreshing) return;

            EditorGUILayout.EndHorizontal();
        }

        protected void BeginTableCol()
        {
            if (isRefreshing) return;

            EditorGUILayout.BeginVertical();
        }

        protected void EndTableCol()
        {
            if (isRefreshing) return;

            EditorGUILayout.EndVertical();
        }

        protected void DrawLabel(string text)
        {
            if (isRefreshing) return;

            EditorGUILayout.LabelField(text);
        }

        protected void DrawTitleLabel(string text)
        {
            if (isRefreshing) return;

            EditorGUILayout.LabelField(text, EditorStyles.boldLabel);
        }

        protected void DrawSection(string text)
        {
            if (isRefreshing) return;

            var style = EditorStyles.largeLabel;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 15;
            style.fixedHeight = 25;
            EditorGUILayout.LabelField(text, style);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        protected void DrawImage(Texture2D image)
        {
            if (isRefreshing) return;

            var centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.alignment = TextAnchor.UpperCenter;
            GUILayout.Label(image, centeredStyle);
        }

        protected void DrawTableCell(string text)
        {
            if (isRefreshing) return;

            EditorGUILayout.TextArea(text, EditorStyles.label);
        }

        protected void DrawTableCell(string text, Color color)
        {
            if (isRefreshing) return;

            var guiStyle = new GUIStyle(EditorStyles.label) {normal = {textColor = color}};
            EditorGUILayout.TextArea(text, guiStyle);
        }

        protected void DrawTableHeader(string text)
        {
            if (isRefreshing) return;

            EditorGUILayout.TextArea(text, EditorStyles.boldLabel);
        }

        protected void DrawInfoBox(string text)
        {
            if (isRefreshing) return;

            EditorGUILayout.HelpBox(text, MessageType.Info);
        }

        protected void DrawWarningBox(string text)
        {
            if (isRefreshing) return;

            EditorGUILayout.HelpBox(text, MessageType.Warning);
        }

        protected void DrawErrorBox(string text)
        {
            if (isRefreshing) return;

            EditorGUILayout.HelpBox(text, MessageType.Error);
        }

        protected void DrawButton(string text, Action actionOnClick)
        {
            if (isRefreshing) return;

            if (GUILayout.Button(text)) actionOnClick();
        }

        protected void DrawDisabledButton(string text)
        {
            if (isRefreshing) return;

            EditorGUI.BeginDisabledGroup(true);
            GUILayout.Button(text);
            EditorGUI.EndDisabledGroup();
        }

        protected Color DrawColorField(string text, Color color)
        {
            return EditorGUILayout.ColorField(text, color);
        }

        private void RefreshIfNeededOrDraw(ICustomProperty property, Action drawAction)
        {
            if (property == null || property.NeedRefresh())
            {
                Initialize();
                isRefreshing = true;
                Repaint();
            }
            else
            {
                drawAction();
            }
        }

        public override void OnInspectorGUI()
        {
            if (isRefreshing && Event.current.type == EventType.Repaint)
            {
                return;
            }

            if (isRefreshing && Event.current.type == EventType.Layout)
            {
                isRefreshing = false;
            }

            serializedObject.Update();
            Draw();
            serializedObject.ApplyModifiedProperties();
        }

        protected abstract void Initialize();

        protected abstract void Draw();

        protected interface ICustomProperty
        {
            void Draw();
            void DrawWithLabel();
            void DrawWithTitleLabel();

            bool NeedRefresh();
        }

        protected sealed class BasicProperty : ICustomProperty
        {
            private readonly SerializedProperty property;

            public BasicProperty(SerializedProperty property)
            {
                this.property = property;
            }

            public void Draw()
            {
                EditorGUILayout.PropertyField(property, GUIContent.none);
                property.serializedObject.ApplyModifiedProperties();
            }

            public void DrawWithLabel()
            {
                EditorGUILayout.PropertyField(property);
                property.serializedObject.ApplyModifiedProperties();
            }

            public void DrawWithTitleLabel()
            {
                EditorGUILayout.LabelField(property.displayName, EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(property, GUIContent.none);
                property.serializedObject.ApplyModifiedProperties();
            }

            public bool IsNull()
            {
                return property.objectReferenceValue == null;
            }

            public bool NeedRefresh()
            {
                return property.NeedRefresh();
            }
        }
        
        protected sealed class GridProperty : ICustomProperty
        {
            private readonly SerializedProperty property;
            private readonly int nbCols;

            public GridProperty(SerializedProperty property,
                                int nbCols)
            {
                this.property = property;
                this.nbCols = nbCols;
            }

            public void Draw()
            {
                EditorGUILayout.BeginVertical();

                var numCol = 0;
                for (var i = 0; i < property.arraySize; i++)
                {
                    if (numCol == 0)
                    {
                        EditorGUILayout.BeginHorizontal();
                    }

                    var element = property.GetArrayElementAtIndex(i);

                    //If it's Texture2D or Sprite, draw the right picker
                    if (property.arrayElementType == "PPtr<$Sprite>")
                    {
                        element.objectReferenceValue = EditorGUILayout.ObjectField("",
                                                                                   element.objectReferenceValue,
                                                                                   typeof(Sprite),
                                                                                   false,
                                                                                   GUILayout.Width(64));
                    }
                    else if (property.arrayElementType == "PPtr<$Texture2D>")
                    {
                        element.objectReferenceValue = EditorGUILayout.ObjectField("",
                                                                                   element.objectReferenceValue,
                                                                                   typeof(Texture2D),
                                                                                   false,
                                                                                   GUILayout.Width(64));
                    }
                    else
                    {
                        EditorGUILayout.ObjectField(element);
                    }

                    numCol++;
                    if (numCol == nbCols)
                    {
                        numCol = 0;
                        EditorGUILayout.EndHorizontal();
                    }
                }

                EditorGUILayout.EndVertical();

                property.serializedObject.ApplyModifiedProperties();
            }

            public void DrawWithLabel()
            {
                EditorGUILayout.LabelField(property.displayName, EditorStyles.boldLabel);
                Draw();
            }

            public void DrawWithTitleLabel()
            {
                EditorGUILayout.LabelField(property.displayName, EditorStyles.boldLabel);
                Draw();
            }

            public bool NeedRefresh()
            {
                return property.NeedRefresh();
            }
        }
    }
}