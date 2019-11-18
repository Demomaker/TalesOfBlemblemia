using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Harmony;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
 { 
     /// <summary>
     /// Controller for game-wide functionalities
     /// Author : Mike Bédard, Jérémie Bertrand, Zacharie Lavigne, Antoine Lessard
     /// </summary>
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


         private LevelLoader levelLoader;
         private DifficultyLevel difficultyLevel;
         private string currentLevelName;

         public Level[] Levels;
         
         public string PreviousLevelName { get; set; }

         public bool PermaDeath => permaDeath;

         public string CurrentLevelName => levelLoader.LoadedLevel;
         public bool AllLevelsCompleted => PreviousLevelName == Levels[Levels.Length - 1].LevelName;

         public DifficultyLevel DifficultyLevel
         {
             get => difficultyLevel;
             set
             {
                 difficultyLevel = value;
                 if (difficultyLevel != DifficultyLevel.Easy)
                 {
                     permaDeath = true;
                 }
             }
         }

         private void Awake()
         {
             levelLoader = Harmony.Finder.LevelLoader;
             gameSettings = Harmony.Finder.GameSettings;
             PreviousLevelName = gameSettings.EmptyString;
             Levels = new Level[]
             {
                 new Level(gameSettings.EmptyString, gameSettings.TutorialSceneName),
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
             levelLoader.FadeToLevel(gameSettings.MainmenuSceneName, LoadSceneMode.Additive);
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
             PreviousLevelName = levelName;
         }
     }

     public enum DifficultyLevel
     {
         Easy,
         Medium,
         Hard
     }
 }
