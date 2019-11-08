using System;
using UnityEditor;
using System.Collections;
using UnityEngine;

namespace Game
{
    [CustomEditor(typeof(LevelController))]
    public class LevelControllerEditor : Editor
    {
        private SerializedProperty levelName;
        private SerializedProperty backgroundMusic;
        private SerializedProperty doNotEnd;
        private SerializedProperty completeIfAllEnemiesDefeated;
        private SerializedProperty completeIfPointAchieved;
        private SerializedProperty completeIfSurvivedCertainNumberOfTurns;
        private SerializedProperty completeIfCertainEnemyDefeated;
        private SerializedProperty defeatIfAllPlayerUnitsDied;
        private SerializedProperty defeatIfProtectedIsKilled;
        private SerializedProperty defeatIfNotCompleteLevelInCertainAmountOfTurns;
        private SerializedProperty pointToAchieve;
        private SerializedProperty targetsToProtect;
        private SerializedProperty enemyToDefeat;
        private SerializedProperty numberOfTurnsBeforeCompletion;
        private SerializedProperty numberOfTurnsBeforeDefeat;
        private SerializedProperty revertWeaponTriangle;

        private void OnEnable()
        {
            levelName = serializedObject.FindProperty("levelName");
            backgroundMusic = serializedObject.FindProperty("backgroundMusic");
            doNotEnd = serializedObject.FindProperty("doNotEnd");
            completeIfAllEnemiesDefeated = serializedObject.FindProperty("completeIfAllEnemiesDefeated");
            completeIfPointAchieved = serializedObject.FindProperty("completeIfPointAchieved");
            completeIfSurvivedCertainNumberOfTurns = serializedObject.FindProperty("completeIfSurvivedCertainNumberOfTurns");
            completeIfCertainEnemyDefeated = serializedObject.FindProperty("completeIfCertainEnemyDefeated");
            defeatIfAllPlayerUnitsDied = serializedObject.FindProperty("defeatIfAllPlayerUnitsDied");
            defeatIfNotCompleteLevelInCertainAmountOfTurns = serializedObject.FindProperty("defeatIfNotCompleteLevelInCertainAmountOfTurns");
            defeatIfProtectedIsKilled = serializedObject.FindProperty("defeatIfProtectedIsKilled");
            pointToAchieve = serializedObject.FindProperty("pointToAchieve");
            targetsToProtect = serializedObject.FindProperty("targetsToProtect");
            enemyToDefeat = serializedObject.FindProperty("enemyToDefeat");
            numberOfTurnsBeforeCompletion = serializedObject.FindProperty("numberOfTurnsBeforeCompletion");
            numberOfTurnsBeforeDefeat = serializedObject.FindProperty("numberOfTurnsBeforeDefeat");
            revertWeaponTriangle = serializedObject.FindProperty("revertWeaponTriangle");

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.LabelField("Dialogue", EditorStyles.boldLabel);

            EditorGUILayout.LabelField("Level", EditorStyles.boldLabel);

            levelName.stringValue = EditorGUILayout.TextField("Level Name", levelName.stringValue);
            backgroundMusic.objectReferenceValue = 
                EditorGUILayout.ObjectField("Background Music", backgroundMusic.objectReferenceValue, typeof(AudioClip),true );
            doNotEnd.boolValue = EditorGUILayout.Toggle("Do Not End Level", doNotEnd.boolValue);
            
            EditorGUILayout.LabelField("Weapon Transformation", EditorStyles.boldLabel);

            revertWeaponTriangle.boolValue = EditorGUILayout.Toggle("Revert Weapon Triangle", revertWeaponTriangle.boolValue);
            

            if (!doNotEnd.boolValue)
            {
                EditorGUILayout.LabelField("Conditions For Level Completion", EditorStyles.boldLabel);
                
                completeIfAllEnemiesDefeated.boolValue =
                    EditorGUILayout.Toggle("Complete If All Enemies Defeated", completeIfAllEnemiesDefeated.boolValue);
                
                completeIfPointAchieved.boolValue = EditorGUILayout.Toggle( "Complete If Point Achieved", completeIfPointAchieved.boolValue);
                if (completeIfPointAchieved.boolValue) pointToAchieve.vector2IntValue = EditorGUILayout.Vector2IntField("Point To Achieve", pointToAchieve.vector2IntValue);

                completeIfSurvivedCertainNumberOfTurns.boolValue = EditorGUILayout.Toggle(
                    "Complete If Survived Certain Number Of Turns", completeIfSurvivedCertainNumberOfTurns.boolValue);
                if (completeIfSurvivedCertainNumberOfTurns.boolValue)
                    numberOfTurnsBeforeCompletion.intValue = EditorGUILayout.IntField("Number Of Turns Before Completion",
                        numberOfTurnsBeforeCompletion.intValue);

                completeIfCertainEnemyDefeated.boolValue =
                    EditorGUILayout.Toggle("Complete If Certain Enemy Defeated", completeIfCertainEnemyDefeated.boolValue);
                if (completeIfCertainEnemyDefeated.boolValue)
                    enemyToDefeat.objectReferenceValue =
                        EditorGUILayout.ObjectField("Enemy to Defeat", enemyToDefeat.objectReferenceValue, typeof(Unit),true );
                
                EditorGUILayout.LabelField("Conditions For Level Defeat", EditorStyles.boldLabel);

                defeatIfAllPlayerUnitsDied.boolValue = EditorGUILayout.Toggle("Defeat If All Player Units Died",
                    defeatIfAllPlayerUnitsDied.boolValue);

                defeatIfNotCompleteLevelInCertainAmountOfTurns.boolValue = EditorGUILayout.Toggle(
                    "Defeat If Not Complete Level In Certain Amount Of Turns",
                    defeatIfNotCompleteLevelInCertainAmountOfTurns.boolValue);
                if (defeatIfNotCompleteLevelInCertainAmountOfTurns.boolValue)
                    numberOfTurnsBeforeDefeat.intValue = EditorGUILayout.IntField("Number Of Turns Before Defeat",
                        numberOfTurnsBeforeDefeat.intValue);

                defeatIfProtectedIsKilled.boolValue =
                    EditorGUILayout.Toggle("Defeat If Protected Is Killed", defeatIfProtectedIsKilled.boolValue);
                if (defeatIfProtectedIsKilled.boolValue)
                    EditorGUILayout.PropertyField(targetsToProtect,true);
            }
            
                
            
            SaveAndReturn();

        }

        private void SaveAndReturn()
        {
            serializedObject.ApplyModifiedProperties();
            return;
        }
    }
}