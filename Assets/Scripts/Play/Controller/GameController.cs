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
         
         private DifficultyLevel difficultyLevel;
         private readonly Dictionary<DifficultyLevel, int> choiceRangePerDifficulty = new Dictionary<DifficultyLevel, int>();
         private int choiceRange;
         private bool permaDeath;
         
         private string previousLevelName;
         private string currentLevelName;
         
         private Coroutine lastLevelCoroutine;
         private string lastLoadedLevelName;

         public Level[] Levels = new Level[]
         {
             new Level("", Constants.TUTORIAL_SCENE_NAME),
             new Level(Constants.TUTORIAL_SCENE_NAME, Constants.JIMSTERBURG_SCENE_NAME),
             new Level(Constants.JIMSTERBURG_SCENE_NAME, Constants.PARABENE_FOREST_SCENE_NAME),
             new Level(Constants.PARABENE_FOREST_SCENE_NAME, Constants.BLEMBURG_CITADEL_SCENE_NAME),
             new Level(Constants.BLEMBURG_CITADEL_SCENE_NAME, Constants.DARK_TOWER_SCENE_NAME),
             new Level(Constants.BLEMBURG_CITADEL_SCENE_NAME, Constants.RINFRET_VILLAGE_SCENE_NAME),
             new Level(Constants.RINFRET_VILLAGE_SCENE_NAME, Constants.TULIP_VALLEY_SCENE_NAME),
             new Level(Constants.TULIP_VALLEY_SCENE_NAME, Constants.MORKTRESS_SCENE_NAME),
             new Level(Constants.DARK_TOWER_SCENE_NAME, Constants.MORKTRESS_SCENE_NAME)
         };
         
         public string PreviousLevelName => previousLevelName;
         public string CurrentLevelName => currentLevelName;
         public string FirstLevelName => Constants.TUTORIAL_SCENE_NAME;
         public bool AllLevelsCompleted => previousLevelName == Levels[Levels.Length - 1].LevelName;

         public DifficultyLevel DifficultyLevel => difficultyLevel;

         private void Awake()
         {
             choiceRange = choiceRangePerDifficulty[difficultyLevel];
             permaDeath = difficultyLevel != DifficultyLevel.Easy;
         }
         
         private void Start()
         {
             SceneManager.LoadSceneAsync(Constants.MAINMENU_SCENE_NAME, LoadSceneMode.Additive);
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

                 if (levelName != Constants.OVERWORLD_SCENE_NAME)
                 {
                     while (!Harmony.Finder.OverWorldController.CanLoadANewLevel)
                     {
                         yield return null;
                     }
                     SceneManager.UnloadSceneAsync(Constants.OVERWORLD_SCENE_NAME);
                     while (SceneManager.GetSceneByName(Constants.OVERWORLD_SCENE_NAME).isLoaded)
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
             this.difficultyLevel = difficultyLevel;
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
