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

         private readonly Dictionary<DifficultyLevel, int> choiceRangePerDifficulty = new Dictionary<DifficultyLevel, int>();

         private LevelLoader levelLoader;
         private DifficultyLevel difficultyLevel;
         private GameSettings gameSettings;
         private int choiceRange;
         private bool permaDeath;

         public Level[] Levels { get; private set; }
         public string PreviousLevelName { get; set; }
         public bool PermaDeath => permaDeath;
         public int ChoiceRange => choiceRange;

         public DifficultyLevel DifficultyLevel
         {
             get => difficultyLevel;
             set
             {
                 difficultyLevel = value;
                 permaDeath = difficultyLevel != DifficultyLevel.Easy;

                 switch (difficultyLevel)
                 {
                     case DifficultyLevel.Easy:
                         choiceRange = choiceForEasy;
                         break;
                     case DifficultyLevel.Medium:
                         choiceRange = choiceForMedium;
                         break;
                     case DifficultyLevel.Hard:
                         choiceRange = choiceForHard;
                         break;
                     default:
                         choiceRange = choiceForMedium;
                         break;
                 }
             }
         }

         public GameController() : this(DifficultyLevel.Easy) { }
         public GameController(DifficultyLevel difficultyLevel)
         {
             DifficultyLevel = difficultyLevel;
             choiceRangePerDifficulty.Add(DifficultyLevel.Easy, choiceForEasy);
             choiceRangePerDifficulty.Add(DifficultyLevel.Medium, choiceForMedium);
             choiceRangePerDifficulty.Add(DifficultyLevel.Hard, choiceForHard);
         }

         private void Awake()
         {
             levelLoader = Harmony.Finder.LevelLoader;
             gameSettings = Harmony.Finder.GameSettings;
             Levels = new[]
             {
                 new Level(gameSettings.EmptyLevelString, gameSettings.SnowyPeaksSceneName),
                 new Level(gameSettings.SnowyPeaksSceneName, gameSettings.JimsterburgSceneName),
                 new Level(gameSettings.JimsterburgSceneName, gameSettings.ParabeneForestSceneName),
                 new Level(gameSettings.ParabeneForestSceneName, gameSettings.BlemburgCitadelSceneName),
                 new Level(gameSettings.BlemburgCitadelSceneName, gameSettings.DarkTowerSceneName),
                 new Level(gameSettings.BlemburgCitadelSceneName, gameSettings.RinfretVillageSceneName),
                 new Level(gameSettings.RinfretVillageSceneName, gameSettings.TulipValleySceneName),
                 new Level(gameSettings.TulipValleySceneName, gameSettings.MorktressSceneName),
                 new Level(gameSettings.DarkTowerSceneName, gameSettings.MorktressSceneName),
                 new Level(gameSettings.MorktressSceneName, gameSettings.EndSceneName) 
             };
             PreviousLevelName = Levels.First(level => level.LevelName == gameSettings.StartingLevelSceneName).PreviousLevel;
             choiceRange = choiceRangePerDifficulty[DifficultyLevel];
             permaDeath = DifficultyLevel != DifficultyLevel.Easy;
         }

         private void Start()
         {
             levelLoader.LoadLevel(gameSettings.MainmenuSceneName, LoadSceneMode.Additive);
         }

         public void OnLevelCompleted(string levelName)
         {
             PreviousLevelName = levelName;
         }
     }
 }
