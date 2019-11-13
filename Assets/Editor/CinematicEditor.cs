using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Game
{
    [CustomEditor(typeof(Cinematic))]
    public class CinematicEditor : Editor
    {
        private SerializedProperty triggerProperty;
        private SerializedProperty actionsProperty;
        private ReorderableList actionsList;

        private void OnEnable()
        {
            actionsProperty = serializedObject.FindProperty("actions");
            triggerProperty = serializedObject.FindProperty("trigger");
            
        actionsList = new ReorderableList(serializedObject, actionsProperty)
        {
            draggable = true,
            displayAdd = true,
            displayRemove = true,
            
            drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "Actions");
            },
            
            drawElementCallback = (rect, index, active, focused) =>
            {
                var element = actionsProperty.GetArrayElementAtIndex(index);
                
                var cinematicActionType = element.FindPropertyRelative("cinematicActionType");
                var name = element.FindPropertyRelative("name");
                var sentence = element.FindPropertyRelative("sentence");
                var targetUnit = element.FindPropertyRelative("targetUnit");
                var damage = element.FindPropertyRelative("damage");
                var cameraTarget = element.FindPropertyRelative("cameraTarget");
                var cameraZoom = element.FindPropertyRelative("cameraZoom");
                var duration = element.FindPropertyRelative("duration");
                var gameObjectToMove = element.FindPropertyRelative("gameObjectToMove");
                var gameObjectTarget = element.FindPropertyRelative("gameObjectTarget");
                var cameraFollow = element.FindPropertyRelative("cameraFollow");
                
                rect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), cinematicActionType);
                rect.y += EditorGUIUtility.singleLineHeight;
                
                switch ((CinematicActionType)cinematicActionType.intValue)
                {
                    case CinematicActionType.Quote:
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), name);
                        rect.y += EditorGUIUtility.singleLineHeight;
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 3), sentence);
                        rect.y += EditorGUIUtility.singleLineHeight * 3;
                        break;
                    case CinematicActionType.CameraMovement:
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), cameraTarget);
                        rect.y += EditorGUIUtility.singleLineHeight;
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), cameraZoom);
                        rect.y += EditorGUIUtility.singleLineHeight;
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), duration);
                        rect.y += EditorGUIUtility.singleLineHeight;
                        break;
                    case CinematicActionType.GameObjectMovement:
                    {
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), gameObjectToMove);
                        rect.y += EditorGUIUtility.singleLineHeight;
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), gameObjectTarget);
                        rect.y += EditorGUIUtility.singleLineHeight;
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), duration);
                        rect.y += EditorGUIUtility.singleLineHeight;
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), cameraFollow);
                        rect.y += EditorGUIUtility.singleLineHeight;
                        if (cameraFollow.boolValue)
                        {
                            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), cameraZoom);
                            rect.y += EditorGUIUtility.singleLineHeight;
                        }

                        break;
                    }
                    case CinematicActionType.Damage:
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), targetUnit);
                        rect.y += EditorGUIUtility.singleLineHeight;
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), damage);
                        rect.y += EditorGUIUtility.singleLineHeight;
                        break;
                }

            },
            elementHeightCallback = index =>
            {
                var element = actionsProperty.GetArrayElementAtIndex(index);
                var cinematicActionType = element.FindPropertyRelative("cinematicActionType");

                switch ((CinematicActionType)cinematicActionType.intValue)
                {
                    case CinematicActionType.Quote:
                        return EditorGUIUtility.singleLineHeight * 7;
                    case CinematicActionType.CameraMovement:
                        return EditorGUIUtility.singleLineHeight * 6;
                    case CinematicActionType.GameObjectMovement:
                        return EditorGUIUtility.singleLineHeight * 8;
                    case CinematicActionType.Damage:
                        return EditorGUIUtility.singleLineHeight * 5;
                    default:
                        return EditorGUIUtility.singleLineHeight * 3;
                }
            },
            
            onAddCallback = list =>
            {
                var index = list.serializedProperty.arraySize;
                list.serializedProperty.arraySize++;
                list.index = index;
                
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                
                var cinematicActionType = element.FindPropertyRelative("cinematicActionType");
                var name = element.FindPropertyRelative("name");
                var sentence = element.FindPropertyRelative("sentence");
                var targetUnit = element.FindPropertyRelative("targetUnit");
                var damage = element.FindPropertyRelative("damage");
                var cameraTarget = element.FindPropertyRelative("cameraTarget");
                var cameraZoom = element.FindPropertyRelative("cameraZoom");
                var duration = element.FindPropertyRelative("duration");
                var gameObjectToMove = element.FindPropertyRelative("gameObjectToMove");
                var gameObjectTarget = element.FindPropertyRelative("gameObjectTarget");
                var cameraFollow = element.FindPropertyRelative("cameraFollow");
                
                cinematicActionType.intValue = (int) CinematicActionType.Quote;
                name.stringValue = "";
                sentence.stringValue = "";
                cameraTarget.objectReferenceValue = null;
                cameraZoom.floatValue = Constants.MIN_CAM_ORTHOGRAPHIC_SIZE;
                duration.floatValue = 0;
                damage.intValue = 0;
                targetUnit.objectReferenceValue = null;
                gameObjectToMove.objectReferenceValue = null;
                gameObjectTarget.objectReferenceValue = null;
                cameraFollow.boolValue = false;
            }
        };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(triggerProperty);
            actionsList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}