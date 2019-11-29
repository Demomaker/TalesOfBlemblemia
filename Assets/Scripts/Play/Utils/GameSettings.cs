using Harmony;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    /// <summary>
    /// Game settings (Variables with constant values that are customizable for the game)
    /// Author : Mike Bédard, Antoine Lessard
    /// </summary>
    [Findable(Game.Tags.GAME_SETTINGS_TAG)]
    public class GameSettings : MonoBehaviour
    {
        private const float RGB_MAX = 255f;

        [Header("Click Hint Text")] 
        [SerializeField] private string confirmMoveToText = "Confirm Move To";
        [SerializeField] private string confirmRecruitText = "Confirm Recruit";
        [SerializeField] private string confirmRestText = "Confirm Rest";
        [SerializeField] private string confirmAttackText = "Confirm Attack";
        [SerializeField] private string confirmHealText = "Confirm Heal";
        [SerializeField] private string moveToText = "Move To";
        [SerializeField] private string recruitText = "Recruit";
        [SerializeField] private string restText = "Rest";
        [SerializeField] private string attackText = "Attack";
        [SerializeField] private string healText = "Heal";
        [SerializeField] private string selectText = "Select";
        [SerializeField] private string deselectText = "Deselect";
        [SerializeField] private string noneText = "";

        [SerializeField] [Header("Unit settings")] private int playerMovementRange = 3;
        [SerializeField] private int enemyMovementRange = 3;
        [SerializeField] private int playerAttackRange = 1;
        [SerializeField] private int enemyAttackRange = 1;
        [SerializeField] private float attackDuration = 0.3f;
        [SerializeField] private float movementDuration = 0.3f;
        [SerializeField] private int numberOfMaximumUnitsForThePlayer = 8;

        [Header("Scene Names")] 
        [SerializeField] private string emptyLevelString = "";
        [SerializeField] private R.E.Scene snowyPeaksScene;
        [SerializeField] private R.E.Scene jimsterburgScene;
        [SerializeField] private R.E.Scene parabeneForestScene;
        [SerializeField] private R.E.Scene blemburgCitadelScene;
        [SerializeField] private R.E.Scene rinfretVillageScene;
        [SerializeField] private R.E.Scene darkTowerScene;
        [SerializeField] private R.E.Scene tulipValleyScene;
        [SerializeField] private R.E.Scene endScene;
        [SerializeField] private R.E.Scene morktressScene;
        [SerializeField] private R.E.Scene overworldScene;
        [SerializeField] private R.E.Scene mainmenuScene;
        [SerializeField] private R.E.Scene startingLevelScene;
        
        [Header("Colors")] 
        [SerializeField] private Color green = Color.green;
        [SerializeField] private Color red = Color.red;
        [SerializeField] private Color gray = Color.gray;
        [SerializeField] private Color darkGreen = new Color(0, 113f/RGB_MAX, 0);
        [SerializeField] private Color darkYellow = new Color(226f/RGB_MAX, 218f/RGB_MAX, 0);
        [SerializeField] private Color darkRed = new Color(159f/RGB_MAX, 0 ,0);
        [SerializeField] private Color paleAlpha = new Color(1,1,1, 0.5f);
        [SerializeField] private Color opaqueAlpha = new Color(1, 1, 1, 1f);

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
        [SerializeField] [Header("Achievements")] private string achievementUnlockedString = "Achievement unlocked!";
        [SerializeField] [Header("Achievement Timing")] private float secondsBeforeTypingStart = 1;
        [SerializeField] private float secondsBeforeTitleCharacterPrint = 0.1f;
        [SerializeField] private float secondsBeforeTextCharacterPrint = 0.05f;
        [SerializeField] private float secondsBeforeNewAchievementShow = 1f;
        [SerializeField] [Header("Achievement Names")] private string completeCampaignOnEasy = "Baby Steps";
        [SerializeField] private string completeCampaignOnMedium = "Now you're getting it!";
        [SerializeField] private string completeCampaignOnHard = "You mad man!";
        [SerializeField] private string defeatBlackKnight = "In my own castle...";
        [SerializeField] private string reachFinalLevelWith8Players = "The Octet";
        [SerializeField] private string finishALevelWithoutUnitLoss = "No one left behind...";
        [SerializeField] private string finishCampaignWithoutUnitLoss = "Great leader";
        [SerializeField] private string saveAllRecruitablesFromAlternatePath = "Champion of the people!";
        [Header("Achievement Descriptions")]
        [SerializeField] private string completeCampaignOnEasyDescription = "Complete the campaign on easy.";
        [SerializeField] private string completeCampaignOnMediumDescription = "Complete the campaign on medium.";
        [SerializeField] private string completeCampaignOnHardDescription = "Complete the campaign on hard.";
        [SerializeField] private string defeatBlackKnightDescription = "Defeated the Black Knight.";
        [SerializeField] private string reachFinalLevelWith8PlayersDescription = "Reached the final level with a full party.";
        [SerializeField] private string finishALevelWithoutUnitLossDescription = "Complete a level without losing a unit";
        [SerializeField] private string finishCampaignWithoutUnitLossDescription = "Complete the campaign without losing a unit";
        [SerializeField] private string saveAllRecruitablesFromAlternatePathDescription = "Recruited all recruitable units.";
        [SerializeField] [Header("Animation and actions")] private string isMoving = "IsMoving";
        [SerializeField] private string isAttacking = "IsAttacking";
        [SerializeField] private string isGoingToDie = "IsGoingToDie";
        [SerializeField] private string isBeingHurt = "IsBeingHurt";
        [SerializeField] private string isDodging = "IsDodging";
        [SerializeField] private string isResting = "IsResting";
        [SerializeField] [Header("Other")] private string nameString = "Name";
        [SerializeField] private string descriptionString = "Description";
        [SerializeField] private string difficultyString = "Difficulty";
        [SerializeField] private string stageString = "Stage";
        [SerializeField] private bool defaultToggleValue = true;
        [SerializeField] private int defaultSliderValue = 100;
        [SerializeField] private string defaultUsername = "Franklem";
        [SerializeField] private int percent = 100;
        [SerializeField] private string audioPath = "Audio";
        [SerializeField] private string darkKnightName = "Dark Knight";

        
        public string ConfirmMoveToText => confirmMoveToText;
        public string ConfirmRecruitText => confirmRecruitText;
        public string ConfirmRestText => confirmRestText;
        public string ConfirmAttackText => confirmAttackText;
        public string ConfirmHealText => confirmHealText;
        public string MoveToText => moveToText;
        public string RecruitText => recruitText;
        public string RestText => restText;
        public string AttackText => attackText;
        public string HealText => healText;
        public string SelectText => selectText;
        public string DeselectText => deselectText;
        public string NoneText => noneText;

        public Color PaleAlpha => paleAlpha;
        public Color OpaqueAlpha => opaqueAlpha;
        public Color Green => green;
        public Color Red => red;
        public Color Gray => gray;
        public Color DarkGreen => darkGreen;
        public Color DarkYellow => darkYellow;
        public Color DarkRed => darkRed;

        public int PlayerMovementRange => playerMovementRange;
        public int EnemyMovementRange => enemyMovementRange;
        public int PlayerAttackRange => playerAttackRange;
        public int EnemyAttackRange => enemyAttackRange;
        public float AttackDuration => attackDuration;
        public float MovementDuration => movementDuration;
        public int NumberOfMaximumUnitsForThePlayer => numberOfMaximumUnitsForThePlayer;

        public string SnowyPeaksSceneName => snowyPeaksScene.ToString();
        public string JimsterburgSceneName => jimsterburgScene.ToString();
        public string ParabeneForestSceneName => parabeneForestScene.ToString();
        public string BlemburgCitadelSceneName => blemburgCitadelScene.ToString();
        public string RinfretVillageSceneName => rinfretVillageScene.ToString();
        public string DarkTowerSceneName => darkTowerScene.ToString();
        public string TulipValleySceneName => tulipValleyScene.ToString();
        public string EndSceneName => endScene.ToString();
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

        public string AchievementUnlockedString => achievementUnlockedString;
        public float SecondsBeforeTypingStart => secondsBeforeTypingStart;
        public float SecondsBeforeTitleCharacterPrint => secondsBeforeTitleCharacterPrint;
        public float SecondsBeforeTextCharacterPrint => secondsBeforeTextCharacterPrint;
        public float SecondsBeforeNewAchievementShow => secondsBeforeNewAchievementShow;
        public string CompleteCampaignOnEasy => completeCampaignOnEasy;
        public string CompleteCampaignOnEasyDescription => completeCampaignOnEasyDescription;
        public string CompleteCampaignOnMedium => completeCampaignOnMedium;
        public string CompleteCampaignOnMediumDescription => completeCampaignOnMediumDescription;
        public string CompleteCampaignOnHard => completeCampaignOnHard;
        public string CompleteCampaignOnHardDescription => completeCampaignOnHardDescription;
        public string DefeatBlackKnight => defeatBlackKnight;
        public string DefeatBlackKnightDescription => defeatBlackKnightDescription;
        public string ReachFinalLevelWith8Players => reachFinalLevelWith8Players;
        public string ReachFinalLevelWith8PlayersDescription => reachFinalLevelWith8PlayersDescription;
        public string FinishALevelWithoutUnitLoss => finishALevelWithoutUnitLoss;
        public string FinishALevelWithoutUnitLossDescription => finishALevelWithoutUnitLossDescription;
        public string FinishCampaignWithoutUnitLoss => finishCampaignWithoutUnitLoss;
        public string FinishCampaignWithoutUnitLossDescription => finishCampaignWithoutUnitLossDescription;

        public string IsMoving => isMoving;
        public string IsAttacking => isAttacking;
        public string IsGoingToDie => isGoingToDie;
        public string IsBeingHurt => isBeingHurt;
        public string IsDodging => isDodging;
        public string IsResting => isResting;

        public string NameString => nameString;
        public string DescriptionString => descriptionString;
        public string DifficultyString => difficultyString;
        public string StageString => stageString;
        public bool DefaultToggleValue => defaultToggleValue;
        public int DefaultSliderValue => defaultSliderValue;
        public string DefaultUsername => defaultUsername;
        public int Percent => percent;
        public string EmptyLevelString => emptyLevelString;
        public string AudioPath => audioPath;
        public string DarkKnightName => darkKnightName;

    }
}