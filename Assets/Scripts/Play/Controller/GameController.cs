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
         private Coroutine lastLevelCoroutine;
         private string lastLoadedLevelName = null;
         private string nameOfLevelCompleted => (LevelsCompleted.Count <= 0) ? null : LevelsCompleted.Last();
         private List<string> tagsOfObjectsToAlwaysKeep = new List<string>();

         public string NameOfLevelCompleted => nameOfLevelCompleted;
         public List<Level> Levels = new List<Level>();
         public List<string> LevelsCompleted = new List<string>();
         public string CurrentLevelName => lastLoadedLevelName; 
         public bool AllLevelsCompleted => Levels.Count == LevelsCompleted.Count;

         public DifficultyLevel DifficultyLevel => difficultyLevel;

         public void Awake()
         {
             choiceRange = choiceRangePerDifficulty[difficultyLevel];
             permaDeath = difficultyLevel != DifficultyLevel.Easy;
             tagsOfObjectsToAlwaysKeep.Add(Tags.SOUND_MANAGER);
             tagsOfObjectsToAlwaysKeep.Add(Tags.GAME_CONTROLLER_TAG);
             tagsOfObjectsToAlwaysKeep.Add(Tags.ACHIEVEMENT_CONTROLLER_TAG);
         }
         private void Start()
         {
             InstantiateLevelList();
             ResetCompletedLevels();
             SceneManager.LoadSceneAsync(Harmony.Finder.GameSettings.MainmenuSceneName, LoadSceneMode.Additive);
         }

         private void InstantiateLevelList()
         {
             Levels = new List<Level>
             {
                 new Level("", Harmony.Finder.GameSettings.Level1SceneName),
                 new Level(Harmony.Finder.GameSettings.Level1SceneName, Harmony.Finder.GameSettings.Level2SceneName),
                 new Level(Harmony.Finder.GameSettings.Level2SceneName, Harmony.Finder.GameSettings.Level3SceneName),
                 new Level(Harmony.Finder.GameSettings.Level3SceneName, Harmony.Finder.GameSettings.Level4SceneName),
                 new Level(Harmony.Finder.GameSettings.Level4SceneName, Harmony.Finder.GameSettings.Level5SceneName),
                 new Level(Harmony.Finder.GameSettings.Level4SceneName, Harmony.Finder.GameSettings.Level6SceneName),
                 new Level(Harmony.Finder.GameSettings.Level6SceneName, Harmony.Finder.GameSettings.Level7SceneName),
                 new Level(Harmony.Finder.GameSettings.Level7SceneName, Harmony.Finder.GameSettings.MorktressSceneName),
                 new Level(Harmony.Finder.GameSettings.Level6SceneName, Harmony.Finder.GameSettings.MorktressSceneName)
             };
         }
         private void ResetCompletedLevels()
         {
             LevelsCompleted.Clear();
         }

         public void UnloadLevel(string levelname)
         {
             StartCoroutine(UnloadLevelCoroutine(levelname));
         }
         
         public void LoadLevel(string levelname)
         {
             if(lastLevelCoroutine != null)
                 StopCoroutine(lastLevelCoroutine);
             lastLevelCoroutine = StartCoroutine(LoadLevelCoroutine(levelname));
             
             if(lastLoadedLevelName != null)
                 UnloadLevel(lastLoadedLevelName);
             lastLoadedLevelName = levelname;
         }
    
         private IEnumerator LoadLevelCoroutine(string levelname)
         {
             if (!SceneManager.GetSceneByName(levelname).isLoaded)
             {
                 if (levelname != Harmony.Finder.GameSettings.OverworldSceneName)
                 {
                     SceneManager.LoadScene(Harmony.Finder.GameSettings.GameUiSceneName, LoadSceneMode.Additive);
                     SceneManager.UnloadSceneAsync(Harmony.Finder.GameSettings.OverworldSceneName);
                 }
                 SceneManager.UnloadSceneAsync(Harmony.Finder.GameSettings.GameUiSceneName);
                 yield return SceneManager.LoadSceneAsync(levelname,LoadSceneMode.Additive);
             }
             SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelname));
         }
         
         private List<GameObject> GetObjectsToAlwaysKeep()
         {
             List<GameObject> gameObjects = new List<GameObject>();
             foreach (string objectTag in tagsOfObjectsToAlwaysKeep)
             {
                 gameObjects.Add(GameObject.FindWithTag(objectTag));
             }
             return gameObjects;
         }

         private IEnumerator UnloadLevelCoroutine(string levelname)
         {
             if (SceneManager.GetSceneByName(levelname).isLoaded)
                 yield return SceneManager.UnloadSceneAsync(levelname);
         }

         public GameController() : this(DifficultyLevel.Easy)
         {
             
         }
         public GameController(DifficultyLevel difficultyLevel)
         {
             this.difficultyLevel = difficultyLevel;
             choiceRangePerDifficulty.Add(DifficultyLevel.Easy, choiceForEasy);
             choiceRangePerDifficulty.Add(DifficultyLevel.Medium, choiceForMedium);
             choiceRangePerDifficulty.Add(DifficultyLevel.Hard, choiceForHard);
         }

     }

     public enum DifficultyLevel
     {
         Easy,
         Medium,
         Hard
     }
 }
