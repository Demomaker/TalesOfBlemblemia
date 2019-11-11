using System;
using Harmony;
using UnityEngine;

namespace Game
{
    //Game settings
    [Findable(Game.Tags.GAME_CONTROLLER_TAG)]
    public class GameSettings : MonoBehaviour
    {
        [SerializeField] [Header("Unit settings")] private int playerMovementRange = 3;
        [SerializeField] private int enemyMovementRange = 3;
        [SerializeField] private int playerAttackRange = 1;
        [SerializeField] private int enemyAttackRange = 1;
        [SerializeField] private float attackDuration = 0.3f;
        [SerializeField] private float movementDuration = 0.3f;
        [SerializeField] private int numberOfRecruitablesOnAlternatePath = 8;
        [SerializeField] [Header("Input settings")] private KeyCode skipComputerTurnKey = KeyCode.Space;
        [SerializeField] [Header("Scene names")] private string level1SceneName = "Tutorial";
        [SerializeField] private string level2SceneName = "Level2";
        [SerializeField] private string level3SceneName = "ParabeneForest";
        [SerializeField] private string level4SceneName = "TulipValley";
        [SerializeField] private string level5SceneName = "DarkTower";
        [SerializeField] private string level6SceneName = "Level6";
        [SerializeField] private string level7SceneName = "Level7";
        [SerializeField] private string morktressSceneName = "Morktress";
        [SerializeField] private string overworldSceneName = "Overworld";
        [SerializeField] private string mainmenuSceneName = "MainMenu";
        [SerializeField] private string gameUiSceneName = "GameUI";
        [SerializeField] private Levels startingLevelName = Levels.Level1;
        /*[SerializeField]*/ private string startingLevelName = "Tutorial";
        [SerializeField] [Header("Saves")] private int saveSlotOne = 1;
        [SerializeField] private int saveSlotTwo = 2;
        [SerializeField] private int saveSlotThree = 3;
        [SerializeField] [Header("Playable characters")] private string franklemName = "Franklem";
        [SerializeField] private string myriamName = "Myriam";
        [SerializeField] private string bramName = "Bram";
        [SerializeField] private string rassName = "Rass";
        [SerializeField] private string ulricName = "Ulric";
        [SerializeField] private string jebediahName = "Jebediah";
        [SerializeField] private string thomasName = "Thomas";
        [SerializeField] private string abrahamName = "Abraham";
        [SerializeField] private string achievementGetString = "Achievement Get!";
        [SerializeField] [Header("Achievement Names")] private string completeCampaignOnEasy = "Baby Steps";
        [SerializeField] private string completeCampaignOnMedium = "Now you're getting it!";
        [SerializeField] private string completeCampaignOnHard = "You mad man!";
        [SerializeField] private string defeatBlackKnight = "In my own castle...";
        [SerializeField] private string reachFinalLevelWith8Players = "The Octet";
        [SerializeField] private string finishALevelWithoutUnitLoss = "No one left behind...";
        [SerializeField] private string finishCampaignWithoutUnitLoss = "Great leader";
        [SerializeField] private string saveAllRecruitablesFromAlternatePath = "Champion of the people!";
        [SerializeField] [Header("Animation and actions")] private string isMoving = "IsMoving";
        [SerializeField] private string isAttacking = "IsAttacking";
        [SerializeField] private string isGoingToDie = "IsGoingToDie";
        [SerializeField] private string isBeingHurt = "IsBeingHurt";
        [SerializeField] private string isDodging = "IsDodging";
        [SerializeField] private string isResting = "IsResting";
        [SerializeField] [Header("Other")] private string nameString = "Name";
        [SerializeField] private string difficultyString = "Difficulty";
        [SerializeField] private string stageString = "Stage";
        [SerializeField] private bool defaultToggleValue = true;
        [SerializeField] private int defaultSliderValue = 100;
        [SerializeField] private string defaultUsername = "Franklem";
        [SerializeField] private int percent = 100;

        public int PlayerMovementRange => playerMovementRange;
        public int EnemyMovementRange => enemyMovementRange;
        public int PlayerAttackRange => playerAttackRange;
        public int EnemyAttackRange => enemyAttackRange;
        public float AttackDuration => attackDuration;
        public float MovementDuration => movementDuration;
        public int NumberOfRecruitablesOnAlternatePath => numberOfRecruitablesOnAlternatePath;
        public KeyCode SkipComputerTurnKey => skipComputerTurnKey;
        public string Level1SceneName => level1SceneName;
        public string Level2SceneName => level2SceneName;
        public string Level3SceneName => level3SceneName;
        public string Level4SceneName => level4SceneName;
        public string Level5SceneName => level5SceneName;
        public string Level6SceneName => level6SceneName;
        public string Level7SceneName => level7SceneName;
        public string MorktressSceneName => morktressSceneName;
        public string OverworldSceneName => overworldSceneName;
        public string MainmenuSceneName => mainmenuSceneName;
        public string GameUiSceneName => gameUiSceneName;
        public string StartingLevelName => startingLevelName;
        public int SaveSlotOne => saveSlotOne;
        public int SaveSlotTwo => saveSlotTwo;
        public int SaveSlotThree => saveSlotThree;
        public string FranklemName => franklemName;
        public string MyriamName => myriamName;
        public string BramName => bramName;
        public string RassName => rassName;
        public string UlricName => ulricName;
        public string JebediahName => jebediahName;
        public string ThomasName => thomasName;
        public string AbrahamName => abrahamName;
        public string AchievementGetString => achievementGetString;
        public string CompleteCampaignOnEasy => completeCampaignOnEasy;
        public string CompleteCampaignOnMedium => completeCampaignOnMedium;
        public string CompleteCampaignOnHard => completeCampaignOnHard;
        public string DefeatBlackKnight => defeatBlackKnight;
        public string ReachFinalLevelWith8Players => reachFinalLevelWith8Players;
        public string FinishALevelWithoutUnitLoss => finishALevelWithoutUnitLoss;
        public string FinishCampaignWithoutUnitLoss => finishCampaignWithoutUnitLoss;
        public string SaveAllRecruitablesFromAlternatePath => saveAllRecruitablesFromAlternatePath;
        public string IsMoving => isMoving;
        public string IsAttacking => isAttacking;
        public string IsGoingToDie => isGoingToDie;
        public string IsBeingHurt => isBeingHurt;
        public string IsDodging => isDodging;
        public string IsResting => isResting;
        public string NameString => nameString;
        public string DifficultyString => difficultyString;
        public string StageString => stageString;
        public bool DefaultToggleValue => defaultToggleValue;
        public int DefaultSliderValue => defaultSliderValue;
        public string DefaultUsername => defaultUsername;
        public int Percent => percent;

        private void Awake()
        {
            
        }

        public string GetLevelNameFromLevel(Level level)
        {
            switch (level)
            {
                case Level.Level1:
                    return Level1SceneName;
                case Level.Level2:
                    return Level2SceneName;
                case Level.Level3:
                    return Level3SceneName;
                case Level.Level4:
                    return Level4SceneName;
                case Level.Level5:
                    return Level5SceneName;
                case Level.Level6:
                    return Level6SceneName;
                case Level.Level7:
                    return Level7SceneName;
                case Level.Level8:
                    return MorktressSceneName;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }

        public enum Level
        {
            Level1,
            Level2,
            Level3,
            Level4,
            Level5,
            Level6,
            Level7,
            Level8
        }
    }
}