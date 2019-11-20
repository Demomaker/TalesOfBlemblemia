using UnityEditor;
using System.Collections.Generic;
using Harmony;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Editor for the Level Controller. Manages the serialized fields for the Level Controller.
    /// Author : Mike Bédard
    /// </summary>
    [CustomEditor(typeof(LevelController))]
    public class LevelControllerEditor : Editor
    {
        /*Property Indexes in Property Names List*/
        private const int FIRST_SERIALIZED_PROPERTY_INDEX_IN_NAME_LIST = 10;
        private const int BACKGROUND_MUSIC_PROPERTY_INDEX = 
            FIRST_SERIALIZED_PROPERTY_INDEX_IN_NAME_LIST;
        private const int DO_NOT_END_PROPERTY_INDEX = 
            FIRST_SERIALIZED_PROPERTY_INDEX_IN_NAME_LIST + 1;
        private const int CUSTOM_OBJECTIVE_MESSAGE_PROPERTY_INDEX = 
            FIRST_SERIALIZED_PROPERTY_INDEX_IN_NAME_LIST + 2;
        private const int COMPLETE_IF_POINT_ACHIEVED_PROPERTY_INDEX = 
            FIRST_SERIALIZED_PROPERTY_INDEX_IN_NAME_LIST + 3;
        private const int COMPLETE_IF_SURVIVED_CERTAIN_NUMBER_OF_TURNS_PROPERTY_INDEX =
            FIRST_SERIALIZED_PROPERTY_INDEX_IN_NAME_LIST + 4;
        private const int COMPLETE_IF_CERTAIN_ENEMY_DEFEATED_PROPERTY_INDEX = 
            FIRST_SERIALIZED_PROPERTY_INDEX_IN_NAME_LIST + 5;
        private const int POINT_TO_ACHIEVE_PROPERTY_INDEX = 
            FIRST_SERIALIZED_PROPERTY_INDEX_IN_NAME_LIST + 6;
        private const int TARGETS_TO_DEFEAT_PROPERTY_INDEX = 
            FIRST_SERIALIZED_PROPERTY_INDEX_IN_NAME_LIST + 7;
        private const int ALL_TARGETS_NEED_TO_BE_DEFEATED_PROPERTY_INDEX = 
            FIRST_SERIALIZED_PROPERTY_INDEX_IN_NAME_LIST + 8;
        private const int TARGETS_TO_PROTECT_PROPERTY_INDEX = 
            FIRST_SERIALIZED_PROPERTY_INDEX_IN_NAME_LIST + 9;
        private const int NUMBER_OF_TURNS_BEFORE_COMPLETION_PROPERTY_INDEX = 
            FIRST_SERIALIZED_PROPERTY_INDEX_IN_NAME_LIST + 10;
        private const int REVERT_WEAPON_TRIANGLE_PROPERTY_INDEX = 
            FIRST_SERIALIZED_PROPERTY_INDEX_IN_NAME_LIST + 11;
        private const int POINTING_ARROW_PREFAB_PROPERTY_INDEX =
            FIRST_SERIALIZED_PROPERTY_INDEX_IN_NAME_LIST + 12;

        /*Header and Field Names*/
        private const string LEVEL_HEADER = "Level";
        private const string WEAPON_TRANSFORMATION_HEADER = "Weapon Transformation";
        private const string LEVEL_COMPLETION_HEADER = "Conditions For Level Completion";
        private const string BACKGROUND_MUSIC_FIELD_NAME = "Background Music";
        private const string DO_NOT_END_FIELD_NAME = "Do Not End Level";
        private const string CUSTOM_OBJECTIVE_MESSAGE_FIELD_NAME = "Custom Objective Message";
        private const string COMPLETE_IF_POINT_ACHIEVED_FIELD_NAME = "Complete If Point Achieved";
        private const string COMPLETE_IF_CERTAIN_ENEMIES_DEFEATED_FIELD_NAME = "Complete If Certain Targets Defeated";
        private const string COMPLETE_IF_SURVIVED_CERTAIN_NUMBER_OF_TURNS_FIELD_NAME = "Complete If Survived Certain Number Of Turns";
        private const string ALL_TARGETS_NEED_TO_BE_DEFEATED_FIELD_NAME = "All Targets Need To Be Defeated";
        private const string POINT_TO_ACHIEVE_FIELD_NAME = "Point To Achieve";
        private const string NUMBER_OF_TURNS_BEFORE_COMPLETION_FIELD_NAME = "Number Of Turns Before Level Completion";
        private const string REVERT_WEAPON_TRIANGLE_FIELD_NAME = "Revert Weapon Triangle";
        private const string POINTING_ARROW_PREFAB_FIELD_NAME = "Pointing Arrow Prefab";
        
        private List<string> serializedPropertyNames;
        
        /*Properties*/
        private SerializedProperty backgroundMusic;
        private SerializedProperty doNotEnd;
        private SerializedProperty customObjectiveMessage;
        private SerializedProperty completeIfPointAchieved;
        private SerializedProperty completeIfSurvivedCertainNumberOfTurns;
        private SerializedProperty completeIfCertainTargetsDefeated;
        private SerializedProperty pointToAchieve;
        private SerializedProperty targetsToProtect;
        private SerializedProperty allTargetsNeedToBeDefeated;
        private SerializedProperty targetsToDefeat;
        private SerializedProperty numberOfTurnsBeforeCompletion;
        private SerializedProperty revertWeaponTriangle;
        private SerializedProperty pointingArrowPrefab;

        private void OnEnable()
        {
            InitializeSerializedPropertyNamesList();
            InitializeSerializedProperties();
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            ShowAndEditProperties();
            serializedObject.ApplyModifiedProperties();
        }

        private void InitializeSerializedPropertyNamesList()
        {
            serializedPropertyNames = new List<string>();
            SerializedProperty serializedProperty = serializedObject.GetIterator();
            serializedProperty.Next(true);
            serializedPropertyNames.Add(serializedProperty.name);
            while (serializedProperty.Next(false))
            {
                serializedPropertyNames.Add(serializedProperty.name);
            }
        }

        private void InitializeSerializedProperties()
        {
            backgroundMusic = GetPropertyAtIndex(BACKGROUND_MUSIC_PROPERTY_INDEX);
            doNotEnd = GetPropertyAtIndex(DO_NOT_END_PROPERTY_INDEX);
            customObjectiveMessage = GetPropertyAtIndex(CUSTOM_OBJECTIVE_MESSAGE_PROPERTY_INDEX);
            completeIfPointAchieved = GetPropertyAtIndex(COMPLETE_IF_POINT_ACHIEVED_PROPERTY_INDEX);
            completeIfSurvivedCertainNumberOfTurns = GetPropertyAtIndex(COMPLETE_IF_SURVIVED_CERTAIN_NUMBER_OF_TURNS_PROPERTY_INDEX);
            completeIfCertainTargetsDefeated = GetPropertyAtIndex(COMPLETE_IF_CERTAIN_ENEMY_DEFEATED_PROPERTY_INDEX);
            pointToAchieve = GetPropertyAtIndex(POINT_TO_ACHIEVE_PROPERTY_INDEX);
            targetsToProtect = GetPropertyAtIndex(TARGETS_TO_PROTECT_PROPERTY_INDEX);
            targetsToDefeat = GetPropertyAtIndex(TARGETS_TO_DEFEAT_PROPERTY_INDEX);
            allTargetsNeedToBeDefeated = GetPropertyAtIndex(ALL_TARGETS_NEED_TO_BE_DEFEATED_PROPERTY_INDEX);
            numberOfTurnsBeforeCompletion = GetPropertyAtIndex(NUMBER_OF_TURNS_BEFORE_COMPLETION_PROPERTY_INDEX);
            revertWeaponTriangle = GetPropertyAtIndex(REVERT_WEAPON_TRIANGLE_PROPERTY_INDEX);
            pointingArrowPrefab = GetPropertyAtIndex(POINTING_ARROW_PREFAB_PROPERTY_INDEX);
        }

        private SerializedProperty GetPropertyAtIndex(int serializedPropertyNamesIndex)
        {
            return serializedObject.FindProperty(serializedPropertyNames[serializedPropertyNamesIndex]);
        }

        private bool DoNotEndLevel()
        {
            return doNotEnd.boolValue;
        }

        private void ShowAndEditProperties()
        {
            ShowAndEditPointingArrowProperty();
            ShowAndEditBasicLevelProperties();
            ShowAndEditWeaponProperties();
            if (DoNotEndLevel()) return;
            ShowAndEditLevelCompletionProperties();
        }

        private void ShowAndEditPointingArrowProperty()
        {
            pointingArrowPrefab.objectReferenceValue =
                EditorGUILayout.ObjectField(POINTING_ARROW_PREFAB_FIELD_NAME, pointingArrowPrefab.objectReferenceValue, typeof(UnityEngine.Object),true );
        }

        private void ShowAndEditBasicLevelProperties()
        {
            EditorGUILayout.LabelField(LEVEL_HEADER, EditorStyles.boldLabel);
            ShowAndEditBackgroundMusic();
            ShowAndEditDoNotEnd();
            ShowAndEditCustomObjectiveMessage();
        }

        private void ShowAndEditWeaponProperties()
        {
            EditorGUILayout.LabelField(WEAPON_TRANSFORMATION_HEADER, EditorStyles.boldLabel);
            ShowAndEditRevertWeaponTriangle();
        }

        private void ShowAndEditLevelCompletionProperties()
        {
            EditorGUILayout.LabelField(LEVEL_COMPLETION_HEADER, EditorStyles.boldLabel);
            ShowAndEditCompleteIfPointAchieved();
            ShowAndEditCompleteIfSurvivedCertainNumberOfTurns();
            ShowAndEditCompleteIfCertainEnemiesDefeated();
        }

        private void ShowAndEditBackgroundMusic()
        {
            backgroundMusic.objectReferenceValue =
                EditorGUILayout.ObjectField(BACKGROUND_MUSIC_FIELD_NAME, backgroundMusic.objectReferenceValue, typeof(AudioClip),true );
        }

        private void ShowAndEditDoNotEnd()
        {
            doNotEnd.boolValue = EditorGUILayout.Toggle(DO_NOT_END_FIELD_NAME, doNotEnd.boolValue);
        }

        private void ShowAndEditCustomObjectiveMessage()
        {
            customObjectiveMessage.stringValue = EditorGUILayout.TextField(CUSTOM_OBJECTIVE_MESSAGE_FIELD_NAME,
                customObjectiveMessage.stringValue);
        }

        private void ShowAndEditRevertWeaponTriangle()
        {
            revertWeaponTriangle.boolValue = EditorGUILayout.Toggle(REVERT_WEAPON_TRIANGLE_FIELD_NAME, revertWeaponTriangle.boolValue);
        }

        private void ShowAndEditCompleteIfPointAchieved()
        {
            completeIfPointAchieved.boolValue = EditorGUILayout.Toggle( COMPLETE_IF_POINT_ACHIEVED_FIELD_NAME, completeIfPointAchieved.boolValue);
            if (completeIfPointAchieved.boolValue) pointToAchieve.vector2IntValue = EditorGUILayout.Vector2IntField(POINT_TO_ACHIEVE_FIELD_NAME, pointToAchieve.vector2IntValue);
        }

        private void ShowAndEditCompleteIfSurvivedCertainNumberOfTurns()
        {
            completeIfSurvivedCertainNumberOfTurns.boolValue = EditorGUILayout.Toggle(
                COMPLETE_IF_SURVIVED_CERTAIN_NUMBER_OF_TURNS_FIELD_NAME, completeIfSurvivedCertainNumberOfTurns.boolValue);
            if (completeIfSurvivedCertainNumberOfTurns.boolValue)
                numberOfTurnsBeforeCompletion.intValue = EditorGUILayout.IntField(NUMBER_OF_TURNS_BEFORE_COMPLETION_FIELD_NAME,
                    numberOfTurnsBeforeCompletion.intValue);
        }

        private void ShowAndEditCompleteIfCertainEnemiesDefeated()
        {
            completeIfCertainTargetsDefeated.boolValue =
                EditorGUILayout.Toggle(COMPLETE_IF_CERTAIN_ENEMIES_DEFEATED_FIELD_NAME, completeIfCertainTargetsDefeated.boolValue);
            if (!completeIfCertainTargetsDefeated.boolValue) return;
            EditorGUILayout.PropertyField(targetsToDefeat,true);
            allTargetsNeedToBeDefeated.boolValue = EditorGUILayout.Toggle(ALL_TARGETS_NEED_TO_BE_DEFEATED_FIELD_NAME,allTargetsNeedToBeDefeated.boolValue);
        }

    }
}