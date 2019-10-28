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
        private SerializedProperty backgroundMusicOptionProperty;
        private SerializedProperty doNotEnd;
        private SerializedProperty completeIfAllEnemiesDefeated;
        private SerializedProperty completeIfPointAchieved;
        private SerializedProperty completeIfSurvivedCertainNumberOfTurns;
        private SerializedProperty completeIfCertainEnemyDefeated;
        private SerializedProperty defeatIfAllPlayerUnitsDied;
        private SerializedProperty defeatIfProtectedIsKilled;
        private SerializedProperty defeatIfNotCompleteLevelInCertainAmountOfTurns;
        private SerializedProperty pointToAchieve;
        private SerializedProperty unitToProtect;
        private SerializedProperty enemyToDefeat;
        private SerializedProperty numberOfTurnsBeforeCompletion;
        private SerializedProperty numberOfTurnsBeforeDefeat;
        private SerializedProperty revertWeaponTriangle;
        private AudioClips audioClips;
        
        LevelBackgroundMusicType backgroundMusicOption;

        private void OnEnable()
        {
            levelName = serializedObject.FindProperty("levelName");
            backgroundMusic = serializedObject.FindProperty("backgroundMusic");
            backgroundMusicOptionProperty = serializedObject.FindProperty("backgroundMusicOption");
            backgroundMusicOption = (LevelBackgroundMusicType)backgroundMusicOptionProperty.enumValueIndex;
            doNotEnd = serializedObject.FindProperty("doNotEnd");
            completeIfAllEnemiesDefeated = serializedObject.FindProperty("completeIfAllEnemiesDefeated");
            completeIfPointAchieved = serializedObject.FindProperty("completeIfPointAchieved");
            completeIfSurvivedCertainNumberOfTurns = serializedObject.FindProperty("completeIfSurvivedCertainNumberOfTurns");
            completeIfCertainEnemyDefeated = serializedObject.FindProperty("completeIfCertainEnemyDefeated");
            defeatIfAllPlayerUnitsDied = serializedObject.FindProperty("defeatIfAllPlayerUnitsDied");
            defeatIfNotCompleteLevelInCertainAmountOfTurns =
                serializedObject.FindProperty("defeatIfNotCompleteLevelInCertainAmountOfTurns");
            defeatIfProtectedIsKilled = serializedObject.FindProperty("defeatIfProtectedIsKilled");
            pointToAchieve = serializedObject.FindProperty("pointToAchieve");
            unitToProtect = serializedObject.FindProperty("unitToProtect");
            enemyToDefeat = serializedObject.FindProperty("enemyToDefeat");
            numberOfTurnsBeforeCompletion = serializedObject.FindProperty("numberOfTurnsBeforeCompletion");
            numberOfTurnsBeforeDefeat = serializedObject.FindProperty("numberOfTurnsBeforeDefeat");
            revertWeaponTriangle = serializedObject.FindProperty("revertWeaponTriangle");
            audioClips = Finder.AudioClips;
            if(audioClips == null) audioClips = new NullAudioClips();

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.LabelField("Dialogue", EditorStyles.boldLabel);

            EditorGUILayout.LabelField("Level", EditorStyles.boldLabel);

            levelName.stringValue = EditorGUILayout.TextField("Level Name", levelName.stringValue);
            backgroundMusicOption = (LevelBackgroundMusicType)EditorGUILayout.EnumPopup("Background Music Type", backgroundMusicOption);
            backgroundMusicOptionProperty.enumValueIndex = (int)backgroundMusicOption;
            switch (backgroundMusicOption)
            {
                case LevelBackgroundMusicType.Forest :
                    backgroundMusic.objectReferenceValue = audioClips.ForestMusic;
                    break;
                case LevelBackgroundMusicType.Castle :
                    backgroundMusic.objectReferenceValue = audioClips.CastleMusic;
                    break;
                case LevelBackgroundMusicType.Boss :
                    backgroundMusic.objectReferenceValue = audioClips.BossMusic;
                    break;
            }
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
                    unitToProtect.objectReferenceValue = EditorGUILayout.ObjectField("Unit To Protect",
                        unitToProtect.objectReferenceValue, typeof(Unit), true);
            }
            
                
            
            SaveAndReturn();

        }

        public void SaveAndReturn()
        {
            serializedObject.ApplyModifiedProperties();
            return;
        }
    }
}