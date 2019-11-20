using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Game
{
    //Author: Jérémie Bertrand
    [CustomEditor(typeof(Cinematic))]
    public class CinematicEditor : Editor
    {

        #region Header and Field Names
        
        private const string REORDERABLE_LIST_HEADER = "Actions";
        private const string CINEMATIC_ACTIONS_FIELD_NAME = "actions";
        private const string CINEMATIC_TRIGGER_FIELD_NAME = "trigger";
        private const string CINEMATIC_ACTION_TYPE_FIELD_NAME = "cinematicActionType";
        private const string DIALOGUE_NAME_FIELD_NAME = "name";
        private const string DIALOGUE_SENTENCE_FIELD_NAME = "sentence";
        private const string TARGET_UNIT_FIELD_NAME = "targetUnit";
        private const string DAMAGE_FIELD_NAME = "damage";
        private const string CAMERA_ZOOM_FIELD_NAME = "cameraZoom";
        private const string CAMERA_TARGET_FIELD_NAME = "cameraTarget";
        private const string GAME_OBJECT_TO_MOVE_FIELD_NAME = "gameObjectToMove";
        private const string GAME_OBJECT_TO_ACTIVATE_FIELD_NAME = "gameObjectToActivate";
        private const string GAME_OBJECT_TARGET_FIELD_NAME = "gameObjectTarget";
        private const string FOLLOW_GAME_OBJECT_WITH_CAMERA_FIELD_NAME = "cameraFollow";
        private const string DURATION_FIELD_NAME = "duration";
        
        #endregion

        #region Properties
        
        private SerializedProperty cinematicTriggerProperty;
        private SerializedProperty cinematicActionsProperty;
        private ReorderableList cinematicActionsReorderableList;
        
        #endregion
        
        private void OnEnable()
        {
            cinematicActionsProperty = serializedObject.FindProperty(CINEMATIC_ACTIONS_FIELD_NAME);
            cinematicTriggerProperty = serializedObject.FindProperty(CINEMATIC_TRIGGER_FIELD_NAME);
            
            cinematicActionsReorderableList = new ReorderableList(serializedObject, cinematicActionsProperty)
            {
                draggable = true,
                displayAdd = true,
                displayRemove = true,
                
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, REORDERABLE_LIST_HEADER);
                },
                
                drawElementCallback = (rect, index, active, focused) =>
                {
                    var element = cinematicActionsProperty.GetArrayElementAtIndex(index);
                    
                    var cinematicActionType = element.FindPropertyRelative(CINEMATIC_ACTION_TYPE_FIELD_NAME);
                    var dialogueName = element.FindPropertyRelative(DIALOGUE_NAME_FIELD_NAME);
                    var dialogueSentence = element.FindPropertyRelative(DIALOGUE_SENTENCE_FIELD_NAME);
                    var targetUnit = element.FindPropertyRelative(TARGET_UNIT_FIELD_NAME);
                    var damage = element.FindPropertyRelative(DAMAGE_FIELD_NAME);
                    var cameraTarget = element.FindPropertyRelative(CAMERA_TARGET_FIELD_NAME);
                    var cameraZoom = element.FindPropertyRelative(CAMERA_ZOOM_FIELD_NAME);
                    var duration = element.FindPropertyRelative(DURATION_FIELD_NAME);
                    var gameObjectToMove = element.FindPropertyRelative(GAME_OBJECT_TO_MOVE_FIELD_NAME);
                    var gameObjectTarget = element.FindPropertyRelative(GAME_OBJECT_TARGET_FIELD_NAME);
                    var followGameObjectWithCamera = element.FindPropertyRelative(FOLLOW_GAME_OBJECT_WITH_CAMERA_FIELD_NAME);
                    var gameObjectToActivate = element.FindPropertyRelative(GAME_OBJECT_TO_ACTIVATE_FIELD_NAME);
                    
                    rect.y += EditorGUIUtility.singleLineHeight;
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), cinematicActionType);
                    rect.y += EditorGUIUtility.singleLineHeight;
                    
                    switch ((CinematicActionType)cinematicActionType.intValue)
                    {
                        case CinematicActionType.Quote:
                            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), dialogueName);
                            rect.y += EditorGUIUtility.singleLineHeight;
                            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 4), dialogueSentence);
                            rect.y += EditorGUIUtility.singleLineHeight * 4;
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
                            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), followGameObjectWithCamera);
                            rect.y += EditorGUIUtility.singleLineHeight;
                            if (followGameObjectWithCamera.boolValue)
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
                        case CinematicActionType.Activate:
                            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), gameObjectToActivate);
                            break;
                    }

                },
                elementHeightCallback = index =>
                {
                    var element = cinematicActionsProperty.GetArrayElementAtIndex(index);
                    var cinematicActionType = element.FindPropertyRelative(CINEMATIC_ACTION_TYPE_FIELD_NAME);

                    switch ((CinematicActionType)cinematicActionType.intValue)
                    {
                        case CinematicActionType.Quote:
                            return EditorGUIUtility.singleLineHeight * 8;
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
                    
                    var cinematicActionType = element.FindPropertyRelative(CINEMATIC_ACTION_TYPE_FIELD_NAME);
                    var dialogueName = element.FindPropertyRelative(DIALOGUE_NAME_FIELD_NAME);
                    var dialogueSentence = element.FindPropertyRelative(DIALOGUE_SENTENCE_FIELD_NAME);
                    var targetUnit = element.FindPropertyRelative(TARGET_UNIT_FIELD_NAME);
                    var damage = element.FindPropertyRelative(DAMAGE_FIELD_NAME);
                    var cameraTarget = element.FindPropertyRelative(CAMERA_TARGET_FIELD_NAME);
                    var cameraZoom = element.FindPropertyRelative(CAMERA_ZOOM_FIELD_NAME);
                    var duration = element.FindPropertyRelative(DURATION_FIELD_NAME);
                    var gameObjectToMove = element.FindPropertyRelative(GAME_OBJECT_TO_MOVE_FIELD_NAME);
                    var gameObjectTarget = element.FindPropertyRelative(GAME_OBJECT_TARGET_FIELD_NAME);
                    var followGameObjectWithCamera = element.FindPropertyRelative(FOLLOW_GAME_OBJECT_WITH_CAMERA_FIELD_NAME);
                    var gameObjectToActivate = element.FindPropertyRelative(GAME_OBJECT_TO_ACTIVATE_FIELD_NAME);
                    
                    cinematicActionType.intValue = (int) CinematicActionType.Quote;
                    dialogueName.stringValue = "";
                    dialogueSentence.stringValue = "";
                    cameraTarget.objectReferenceValue = null;
                    cameraZoom.floatValue = CameraConstants.MIN_CAM_ORTHOGRAPHIC_SIZE;
                    duration.floatValue = 0;
                    damage.intValue = 0;
                    targetUnit.objectReferenceValue = null;
                    gameObjectToMove.objectReferenceValue = null;
                    gameObjectTarget.objectReferenceValue = null;
                    followGameObjectWithCamera.boolValue = false;
                    gameObjectToActivate.objectReferenceValue = null;
                }
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(cinematicTriggerProperty);
            cinematicActionsReorderableList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}