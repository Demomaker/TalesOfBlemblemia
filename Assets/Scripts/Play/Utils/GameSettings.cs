using Harmony;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Game settings (Variables with constant values that are customizable for the game)
    /// Author : Mike Bédard
    /// </summary>
    [Findable(Game.Tags.GAME_SETTINGS_TAG)]
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

        [Header("Scene names")] 
        [SerializeField] private string emptyLevelString = "";
        [SerializeField] private R.E.Scene tutorialScene;
        [SerializeField] private R.E.Scene jimsterburgScene;
        [SerializeField] private R.E.Scene parabeneForestScene;
        [SerializeField] private R.E.Scene blemburgCitadelScene;
        [SerializeField] private R.E.Scene rinfretVillageScene;
        [SerializeField] private R.E.Scene darkTowerScene;
        [SerializeField] private R.E.Scene tulipValleyScene;
        [SerializeField] private R.E.Scene morktressScene;
        [SerializeField] private R.E.Scene overworldScene;
        [SerializeField] private R.E.Scene mainmenuScene;
        [SerializeField] private R.E.Scene startingLevelScene;

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
        [SerializeField] [Header("Achievements")] private string achievementGetString = "Achievement Completed!";
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

        private static readonly Color paleAlpha = new Color(1,1,1, 0.5f);
        private static readonly Color opaqueAlpha = new Color(1,1,1,1f);

        public Color PaleAlpha => paleAlpha;
        public Color OpaqueAlpha => opaqueAlpha;
        public int PlayerMovementRange => playerMovementRange;
        public int EnemyMovementRange => enemyMovementRange;
        public int PlayerAttackRange => playerAttackRange;
        public int EnemyAttackRange => enemyAttackRange;
        public float AttackDuration => attackDuration;
        public float MovementDuration => movementDuration;
        public int NumberOfRecruitablesOnAlternatePath => numberOfRecruitablesOnAlternatePath;
        public KeyCode SkipComputerTurnKey => skipComputerTurnKey;
        public string TutorialSceneName => tutorialScene.ToString();
        public string JimsterburgSceneName => jimsterburgScene.ToString();
        public string ParabeneForestSceneName => parabeneForestScene.ToString();
        public string BlemburgCitadelSceneName => blemburgCitadelScene.ToString();
        public string RinfretVillageSceneName => rinfretVillageScene.ToString();
        public string DarkTowerSceneName => darkTowerScene.ToString();
        public string TulipValleySceneName => tulipValleyScene.ToString();
        public string MorktressSceneName => morktressScene.ToString();
        public string OverworldSceneName => overworldScene.ToString();
        public string MainmenuSceneName => mainmenuScene.ToString();

        public string StartingLevelSceneName => startingLevelScene.ToString();

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
        public string EmptyLevelString => emptyLevelString;
    }
}