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
         private string startingLevelName = Constants.LEVEL_4_SCENE_NAME;
         private Coroutine lastLevelCoroutine;
         private string lastLoadedLevelName = null;
         private string nameOfLevelCompleted => (LevelsCompleted.Count <= 0) ? null : LevelsCompleted.Last();
         private List<string> tagsOfObjectsToAlwaysKeep = new List<string>();

         public string NameOfLevelCompleted => nameOfLevelCompleted;
         public List<Level> Levels = new List<Level>();
         public List<string> LevelsCompleted = new List<string>();
         public string CurrentLevelName => lastLoadedLevelName; 
         public string StartingLevelName => startingLevelName;
         public bool AllLevelsCompleted => Levels.Count == LevelsCompleted.Count;

         public DifficultyLevel DifficultyLevel => difficultyLevel;

         private void Start()
         {
             InstantiateLevelList();
             ResetCompletedLevels();
             SceneManager.LoadSceneAsync(Constants.MAINMENU_SCENE_NAME, LoadSceneMode.Additive);
         }

         private void InstantiateLevelList()
         {
             Levels = new List<Level>
             {
                 new Level("", Constants.LEVEL_1_SCENE_NAME),
                 new Level(Constants.LEVEL_1_SCENE_NAME, Constants.LEVEL_2_SCENE_NAME),
                 new Level(Constants.LEVEL_2_SCENE_NAME, Constants.LEVEL_3_SCENE_NAME),
                 new Level(Constants.LEVEL_3_SCENE_NAME, Constants.LEVEL_4_SCENE_NAME),
                 new Level(Constants.LEVEL_4_SCENE_NAME, Constants.LEVEL_5_SCENE_NAME),
                 new Level(Constants.LEVEL_4_SCENE_NAME, Constants.LEVEL_6_SCENE_NAME),
                 new Level(Constants.LEVEL_6_SCENE_NAME, Constants.LEVEL_7_SCENE_NAME),
                 new Level(Constants.LEVEL_7_SCENE_NAME, Constants.LEVEL_8_SCENE_NAME),
                 new Level(Constants.LEVEL_6_SCENE_NAME, Constants.LEVEL_8_SCENE_NAME)
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
                 if (levelname != Constants.OVERWORLD_SCENE_NAME)
                 {
                     SceneManager.LoadScene(Constants.GAME_UI_SCENE_NAME, LoadSceneMode.Additive);
                     SceneManager.UnloadSceneAsync(Constants.OVERWORLD_SCENE_NAME);
                 }
                 SceneManager.UnloadSceneAsync(Constants.GAME_UI_SCENE_NAME);
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

         public void Awake()
         {
             choiceRange = choiceRangePerDifficulty[difficultyLevel];
             permaDeath = difficultyLevel != DifficultyLevel.Easy;
             tagsOfObjectsToAlwaysKeep.Add(Tags.SOUND_MANAGER);
             tagsOfObjectsToAlwaysKeep.Add(Tags.GAME_CONTROLLER_TAG);
             tagsOfObjectsToAlwaysKeep.Add(Tags.ACHIEVEMENT_CONTROLLER_TAG);
         }
     }

     public enum DifficultyLevel
     {
         Easy,
         Medium,
         Hard
     }
 }
