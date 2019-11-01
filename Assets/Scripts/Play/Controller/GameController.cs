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
         private string startingLevelName = Constants.LEVEL_3_SCENE_NAME;
         private Coroutine lastLevelCoroutine;
         private string lastLoadedLevelName = null;
         private string nameOfLevelCompleted => (LevelsCompleted.Count <= 0) ? null : LevelsCompleted.Last();

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

         public void UnloadLevel(string levelName)
         {
             StartCoroutine(UnloadLevelCoroutine(levelName));
         }
         
         public void LoadLevel(string levelName)
         {
             if(lastLoadedLevelName != null)
                 UnloadLevel(lastLoadedLevelName);
             lastLoadedLevelName = levelName;
             if(lastLevelCoroutine != null)
                 StopCoroutine(lastLevelCoroutine);
             lastLevelCoroutine = StartCoroutine(LoadLevelCoroutine(levelName));
         }
    
         private IEnumerator LoadLevelCoroutine(string levelName)
         {
             if(!SceneManager.GetSceneByName(levelName).isLoaded)
                 yield return SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
             if (!SceneManager.GetSceneByName(Constants.GAME_UI_SCENE_NAME).isLoaded)
                 yield return SceneManager.LoadSceneAsync(Constants.GAME_UI_SCENE_NAME, LoadSceneMode.Additive);
             SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelName));
         }

         private IEnumerator UnloadLevelCoroutine(string levelName)
         {
             if (SceneManager.GetSceneByName(levelName).isLoaded)
                 yield return SceneManager.UnloadSceneAsync(levelName);
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
             DontDestroyOnLoad(gameObject);
         }
     }

     public enum DifficultyLevel
     {
         Easy,
         Medium,
         Hard
     }
 }
