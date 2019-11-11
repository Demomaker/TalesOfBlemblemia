using System;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace Game
{
    [CustomEditor(typeof(LevelController))]
    public class LevelControllerEditor : Editor
    {
        /*Property Indexes in Property Names List*/
        private const int FIRST_SERIALIZED_PROPERTY_INDEX_IN_NAME_LIST = 10;
        private const int BACKGROUND_MUSIC_PROPERTY_INDEX = FIRST_SERIALIZED_PROPERTY_INDEX_IN_NAME_LIST + 1;
        private const int DO_NOT_END_PROPERTY_INDEX = FIRST_SERIALIZED_PROPERTY_INDEX_IN_NAME_LIST + 3;
        private const int COMPLETE_IF_ALL_ENEMIES_DEFEATED_PROPERTY_INDEX = FIRST_SERIALIZED_PROPERTY_INDEX_IN_NAME_LIST + 4;
        private const int COMPLETE_IF_POINT_ACHIEVED_PROPERTY_INDEX = FIRST_SERIALIZED_PROPERTY_INDEX_IN_NAME_LIST + 5;
        private const int COMPLETE_IF_SURVIVED_CERTAIN_NUMBER_OF_TURNS_PROPERTY_INDEX =
            FIRST_SERIALIZED_PROPERTY_INDEX_IN_NAME_LIST + 6;
        private const int COMPLETE_IF_CERTAIN_ENEMY_DEFEATED_PROPERTY_INDEX = FIRST_SERIALIZED_PROPERTY_INDEX_IN_NAME_LIST + 7;
        private const int DEFEAT_IF_ALL_PLAYER_UNITS_DIED_PROPERTY_INDEX = FIRST_SERIALIZED_PROPERTY_INDEX_IN_NAME_LIST + 8;
        private const int DEFEAT_IF_NOT_COMPLETE_LEVEL_IN_CERTAIN_AMOUNT_OF_TURNS_PROPERTY_INDEX =
            FIRST_SERIALIZED_PROPERTY_INDEX_IN_NAME_LIST + 9;
        private const int DEFEAT_IF_PROTECTED_IS_KILLED_PROPERTY_INDEX = FIRST_SERIALIZED_PROPERTY_INDEX_IN_NAME_LIST + 10;
        private const int POINT_TO_ACHIEVE_PROPERTY_INDEX = FIRST_SERIALIZED_PROPERTY_INDEX_IN_NAME_LIST + 11;
        private const int ENEMY_TO_DEFEAT_PROPERTY_INDEX = FIRST_SERIALIZED_PROPERTY_INDEX_IN_NAME_LIST + 12;
        private const int TARGETS_TO_PROTECT_PROPERTY_INDEX = FIRST_SERIALIZED_PROPERTY_INDEX_IN_NAME_LIST + 13;
        private const int NUMBER_OF_TURNS_BEFORE_COMPLETION_PROPERTY_INDEX = FIRST_SERIALIZED_PROPERTY_INDEX_IN_NAME_LIST + 14;
        private const int NUMBER_OF_TURNS_BEFORE_DEFEAT_PROPERTY_INDEX = FIRST_SERIALIZED_PROPERTY_INDEX_IN_NAME_LIST + 15;
        private const int REVERT_WEAPON_TRIANGLE_PROPERTY_INDEX = FIRST_SERIALIZED_PROPERTY_INDEX_IN_NAME_LIST + 16;

        /*Header and Field Names*/
        private const string LEVEL_HEADER = "Level";
        private const string WEAPON_TRANSFORMATION_HEADER = "Weapon Transformation";
        private const string LEVEL_COMPLETION_HEADER = "Conditions For Level Completion";
        private const string LEVEL_DEFEAT_HEADER = "Conditions For Level Defeat";
        private const string LEVEL_NAME_FIELD_NAME = "Level Name";
        private const string BACKGROUND_MUSIC_FIELD_NAME = "Background Music";
        private const string DO_NOT_END_FIELD_NAME = "Do Not End Level";
        private const string COMPLETE_IF_ALL_ENEMIES_DEFEATED_FIELD_NAME = "Complete If All Enemies Defeated";
        private const string COMPLETE_IF_POINT_ACHIEVED_FIELD_NAME = "Complete If Point Achieved";
        private const string COMPLETE_IF_CERTAIN_ENEMIES_DEFEATED_FIELD_NAME = "Complete If Certain Enemies Defeated";
        private const string COMPLETE_IF_SURVIVED_CERTAIN_NUMBER_OF_TURNS_FIELD_NAME = "Complete If Survived Certain Number Of Turns";
        private const string DEFEAT_IF_ALL_PLAYER_UNITS_DIED_FIELD_NAME = "Defeat If All Player Units Die";
        private const string DEFEAT_IF_PROTECTED_IS_KILLED_FIELD_NAME = "Defeat If Protected Unit Is Killed";
        private const string DEFEAT_IF_NOT_COMPLETE_LEVEL_IN_CERTAIN_AMOUNT_OF_TURNS_FIELD_NAME = "Defeat If Level Not Completed In Certain Amount Of Turns";
        private const string POINT_TO_ACHIEVE_FIELD_NAME = "Point To Achieve";
        private const string ENEMIES_TO_DEFEAT_FIELD_NAME = "Enemies To Defeat";
        private const string NUMBER_OF_TURNS_BEFORE_COMPLETION_FIELD_NAME = "Number Of Turns Before Level Completion";
        private const string NUMBER_OF_TURNS_BEFORE_DEFEAT_FIELD_NAME = "Number Of Turns Before Level Defeat";
        private const string REVERT_WEAPON_TRIANGLE_FIELD_NAME = "Revert Weapon Triangle";
        
        private List<string> serializedPropertyNames;
        
        /*Properties*/
        private SerializedProperty levelName;
        private SerializedProperty backgroundMusic;
        private SerializedProperty doNotEnd;
        private SerializedProperty completeIfAllEnemiesDefeated;
        private SerializedProperty completeIfPointAchieved;
        private SerializedProperty completeIfSurvivedCertainNumberOfTurns;
        private SerializedProperty completeIfCertainEnemiesDefeated;
        private SerializedProperty defeatIfAllPlayerUnitsDied;
        private SerializedProperty defeatIfProtectedIsKilled;
        private SerializedProperty defeatIfNotCompleteLevelInCertainAmountOfTurns;
        private SerializedProperty pointToAchieve;
        private SerializedProperty targetsToProtect;
        private SerializedProperty enemiesToDefeat;
        private SerializedProperty numberOfTurnsBeforeCompletion;
        private SerializedProperty numberOfTurnsBeforeDefeat;
        private SerializedProperty revertWeaponTriangle;

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
            levelName = serializedObject.FindProperty(serializedPropertyNames[FIRST_SERIALIZED_PROPERTY_INDEX_IN_NAME_LIST]);
            backgroundMusic = serializedObject.FindProperty(serializedPropertyNames[BACKGROUND_MUSIC_PROPERTY_INDEX]);
            doNotEnd = serializedObject.FindProperty(serializedPropertyNames[DO_NOT_END_PROPERTY_INDEX]);
            completeIfAllEnemiesDefeated = serializedObject.FindProperty(serializedPropertyNames[COMPLETE_IF_ALL_ENEMIES_DEFEATED_PROPERTY_INDEX]);
            completeIfPointAchieved = serializedObject.FindProperty(serializedPropertyNames[COMPLETE_IF_POINT_ACHIEVED_PROPERTY_INDEX]);
            completeIfSurvivedCertainNumberOfTurns = serializedObject.FindProperty(serializedPropertyNames[COMPLETE_IF_SURVIVED_CERTAIN_NUMBER_OF_TURNS_PROPERTY_INDEX]);
            completeIfCertainEnemiesDefeated = serializedObject.FindProperty(serializedPropertyNames[COMPLETE_IF_CERTAIN_ENEMY_DEFEATED_PROPERTY_INDEX]);
            defeatIfAllPlayerUnitsDied = serializedObject.FindProperty(serializedPropertyNames[DEFEAT_IF_ALL_PLAYER_UNITS_DIED_PROPERTY_INDEX]);
            defeatIfNotCompleteLevelInCertainAmountOfTurns = serializedObject.FindProperty(serializedPropertyNames[DEFEAT_IF_NOT_COMPLETE_LEVEL_IN_CERTAIN_AMOUNT_OF_TURNS_PROPERTY_INDEX]);
            defeatIfProtectedIsKilled = serializedObject.FindProperty(serializedPropertyNames[DEFEAT_IF_PROTECTED_IS_KILLED_PROPERTY_INDEX]);
            pointToAchieve = serializedObject.FindProperty(serializedPropertyNames[POINT_TO_ACHIEVE_PROPERTY_INDEX]);
            targetsToProtect = serializedObject.FindProperty(serializedPropertyNames[TARGETS_TO_PROTECT_PROPERTY_INDEX]);
            enemiesToDefeat = serializedObject.FindProperty(serializedPropertyNames[ENEMY_TO_DEFEAT_PROPERTY_INDEX]);
            numberOfTurnsBeforeCompletion = serializedObject.FindProperty(serializedPropertyNames[NUMBER_OF_TURNS_BEFORE_COMPLETION_PROPERTY_INDEX]);
            numberOfTurnsBeforeDefeat = serializedObject.FindProperty(serializedPropertyNames[NUMBER_OF_TURNS_BEFORE_DEFEAT_PROPERTY_INDEX]);
            revertWeaponTriangle = serializedObject.FindProperty(serializedPropertyNames[REVERT_WEAPON_TRIANGLE_PROPERTY_INDEX]);
        }

        private bool DoNotEndLevel()
        {
            return doNotEnd.boolValue;
        }

        private void ShowAndEditProperties()
        {
            ShowAndEditBasicLevelProperties();
            ShowAndEditWeaponProperties();
            if (DoNotEndLevel()) return;
            ShowAndEditLevelCompletionProperties();
            ShowAndEditLevelDefeatProperties();
        }

        private void ShowAndEditBasicLevelProperties()
        {
            EditorGUILayout.LabelField(LEVEL_HEADER, EditorStyles.boldLabel);
            ShowAndEditLevelName();
            ShowAndEditBackgroundMusic();
            ShowAndEditDoNotEnd();
        }

        private void ShowAndEditWeaponProperties()
        {
            EditorGUILayout.LabelField(WEAPON_TRANSFORMATION_HEADER, EditorStyles.boldLabel);
            ShowAndEditRevertWeaponTriangle();
        }

        private void ShowAndEditLevelCompletionProperties()
        {
            EditorGUILayout.LabelField(LEVEL_COMPLETION_HEADER, EditorStyles.boldLabel);
            ShowAndEditCompleteIfAllEnemiesDefeated();   
            ShowAndEditCompleteIfPointAchieved();
            ShowAndEditCompleteIfSurvivedCertainNumberOfTurns();
            ShowAndEditCompleteIfCertainEnemiesDefeated();
        }

        private void ShowAndEditLevelDefeatProperties()
        {
            EditorGUILayout.LabelField(LEVEL_DEFEAT_HEADER, EditorStyles.boldLabel);
            ShowAndEditDefeatIfAllPlayerUnitsDied();
            ShowAndEditDefeatIfNotCompleteLevelInCertainAmountOfTurns();
            ShowAndEditDefeatIfProtectedIsKilled();
        }

        private void ShowAndEditLevelName()
        {
            levelName.stringValue = EditorGUILayout.TextField(LEVEL_NAME_FIELD_NAME, levelName.stringValue);
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

        private void ShowAndEditRevertWeaponTriangle()
        {
            revertWeaponTriangle.boolValue = EditorGUILayout.Toggle(REVERT_WEAPON_TRIANGLE_FIELD_NAME, revertWeaponTriangle.boolValue);
        }

        private void ShowAndEditCompleteIfAllEnemiesDefeated()
        {
            completeIfAllEnemiesDefeated.boolValue =
                EditorGUILayout.Toggle(COMPLETE_IF_ALL_ENEMIES_DEFEATED_FIELD_NAME, completeIfAllEnemiesDefeated.boolValue);
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
            completeIfCertainEnemiesDefeated.boolValue =
                EditorGUILayout.Toggle(COMPLETE_IF_CERTAIN_ENEMIES_DEFEATED_FIELD_NAME, completeIfCertainEnemiesDefeated.boolValue);
            if (completeIfCertainEnemiesDefeated.boolValue) EditorGUILayout.PropertyField(enemiesToDefeat,true);
        }

        private void ShowAndEditDefeatIfAllPlayerUnitsDied()
        {
            defeatIfAllPlayerUnitsDied.boolValue = EditorGUILayout.Toggle(DEFEAT_IF_ALL_PLAYER_UNITS_DIED_FIELD_NAME,
                defeatIfAllPlayerUnitsDied.boolValue);
        }

        private void ShowAndEditDefeatIfNotCompleteLevelInCertainAmountOfTurns()
        {
            defeatIfNotCompleteLevelInCertainAmountOfTurns.boolValue = EditorGUILayout.Toggle(
                DEFEAT_IF_NOT_COMPLETE_LEVEL_IN_CERTAIN_AMOUNT_OF_TURNS_FIELD_NAME,
                defeatIfNotCompleteLevelInCertainAmountOfTurns.boolValue);
            if (defeatIfNotCompleteLevelInCertainAmountOfTurns.boolValue)
                numberOfTurnsBeforeDefeat.intValue = EditorGUILayout.IntField(NUMBER_OF_TURNS_BEFORE_DEFEAT_FIELD_NAME,
                    numberOfTurnsBeforeDefeat.intValue);
        }

        private void ShowAndEditDefeatIfProtectedIsKilled()
        {
            defeatIfProtectedIsKilled.boolValue =
                EditorGUILayout.Toggle(DEFEAT_IF_PROTECTED_IS_KILLED_FIELD_NAME, defeatIfProtectedIsKilled.boolValue);
            if (defeatIfProtectedIsKilled.boolValue)
                EditorGUILayout.PropertyField(targetsToProtect,true);
        }

    }
}