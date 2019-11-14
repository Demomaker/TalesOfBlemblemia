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
         
         private readonly Dictionary<DifficultyLevel, int> choiceRangePerDifficulty = new Dictionary<DifficultyLevel, int>();
         
         private LevelLoader levelLoader;
         private DifficultyLevel difficultyLevel;
         private GameSettings gameSettings;
         private Coroutine lastLevelCoroutine;
         private string previousLevelName;
         private string currentLevelName;
         private string lastLoadedLevelName;
         private int choiceRange;
         private bool permaDeath;



         public DifficultyLevel DifficultyLevel => difficultyLevel;
         public Level[] Levels { get; private set; }
         public string PreviousLevelName => previousLevelName;
         public string CurrentLevelName => currentLevelName;
         public string FirstLevelName => gameSettings.TutorialSceneName;
         public bool AllLevelsCompleted => previousLevelName == Levels[Levels.Length - 1].LevelName;


         private void Awake()
         {
             levelLoader = Harmony.Finder.LevelLoader;
             gameSettings = Harmony.Finder.GameSettings;
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
             choiceRange = choiceRangePerDifficulty[difficultyLevel];
             permaDeath = difficultyLevel != DifficultyLevel.Easy;
         }
         
         private void Start()
         {
             levelLoader.FadeToLevel(gameSettings.MainmenuSceneName, LoadSceneMode.Additive);
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
