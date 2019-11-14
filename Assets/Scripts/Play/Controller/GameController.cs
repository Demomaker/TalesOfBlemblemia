using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Harmony;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
 { 
     [Findable("GameController")]
     public class GameController : MonoBehaviour
     {
         [SerializeField] private int choiceForEasy = 10;
         [SerializeField] private int choiceForMedium = 5;
         [SerializeField] private int choiceForHard = 3;
         
         private GameSettings gameSettings;
         private readonly Dictionary<DifficultyLevel, int> choiceRangePerDifficulty = new Dictionary<DifficultyLevel, int>();
         private int choiceRange;
         private bool permaDeath;

         private string previousLevelName;
         private string currentLevelName;
         
         private Coroutine lastLevelCoroutine;
         private string lastLoadedLevelName;

         public Level[] Levels;
         
         public string PreviousLevelName => previousLevelName;
         public string CurrentLevelName => currentLevelName;
         public bool AllLevelsCompleted => previousLevelName == Levels[Levels.Length - 1].LevelName;

         public DifficultyLevel DifficultyLevel { get; set; }

         private void Awake()
         {
             gameSettings = Harmony.Finder.GameSettings;
             previousLevelName = gameSettings.JimsterburgSceneName;
             Levels = new Level[]
             {
                 new Level("", gameSettings.TutorialSceneName),
                 new Level(gameSettings.TutorialSceneName, gameSettings.JimsterburgSceneName),
                 new Level(gameSettings.JimsterburgSceneName, gameSettings.ParabeneForestSceneName),
                 new Level(gameSettings.ParabeneForestSceneName, gameSettings.BlemburgCitadelSceneName),
                 new Level(gameSettings.BlemburgCitadelSceneName, gameSettings.DarkTowerSceneName),
                 new Level(gameSettings.BlemburgCitadelSceneName, gameSettings.RinfretVillageSceneName),
                 new Level(gameSettings.RinfretVillageSceneName, gameSettings.TulipValleySceneName),
                 new Level(gameSettings.TulipValleySceneName, gameSettings.MorktressSceneName),
                 new Level(gameSettings.DarkTowerSceneName, gameSettings.MorktressSceneName)
             };
             choiceRange = choiceRangePerDifficulty[DifficultyLevel];
             permaDeath = DifficultyLevel != DifficultyLevel.Easy;
         }
         
         private void Start()
         {
             SceneManager.LoadSceneAsync(gameSettings.MainmenuSceneName, LoadSceneMode.Additive);
         }

         private void UnloadLevel(string levelName)
         {
             StartCoroutine(UnloadLevelCoroutine(levelName));
         }
         
         public void LoadLevel(string levelName)
         {
             StartCoroutine(LoadLevelCoroutine(levelName));
         }

         private IEnumerator LoadLevelCoroutine(string levelName)
         {
             if (!SceneManager.GetSceneByName(levelName).isLoaded)
             {
                 var levelScene = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
                 levelScene.allowSceneActivation = false;
                 while (levelScene.progress < 0.9f)
                 {
                     yield return null;
                 }

                 if (levelName != gameSettings.OverworldSceneName)
                 {
                     while (!Harmony.Finder.OverWorldController.CanLoadANewLevel)
                     {
                         yield return null;
                     }
                     SceneManager.UnloadSceneAsync(gameSettings.OverworldSceneName);
                     while (SceneManager.GetSceneByName(gameSettings.OverworldSceneName).isLoaded)
                     {
                         yield return null;
                     }
                 }
                 else if (!string.IsNullOrEmpty(currentLevelName))
                 {
                     SceneManager.UnloadSceneAsync(currentLevelName);
                     while (SceneManager.GetSceneByName(currentLevelName).isLoaded)
                     {
                         yield return null;
                     }
                 }
                 levelScene.allowSceneActivation = true;
                 currentLevelName = levelName;
             }
         }

         private IEnumerator UnloadLevelCoroutine(string levelName)
         {
             if (SceneManager.GetSceneByName(levelName).isLoaded)
                 yield return SceneManager.UnloadSceneAsync(levelName);
         }

         public GameController() : this(DifficultyLevel.Easy) { }
         public GameController(DifficultyLevel difficultyLevel)
         {
             DifficultyLevel = difficultyLevel;
             choiceRangePerDifficulty.Add(DifficultyLevel.Easy, choiceForEasy);
             choiceRangePerDifficulty.Add(DifficultyLevel.Medium, choiceForMedium);
             choiceRangePerDifficulty.Add(DifficultyLevel.Hard, choiceForHard);
         }

         public void OnLevelCompleted(string levelName)
         {
             previousLevelName = levelName;
         }
     }

     public enum DifficultyLevel
     {
         Easy,
         Medium,
         Hard
     }
 }
