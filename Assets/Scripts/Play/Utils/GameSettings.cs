using System;
using Harmony;
using UnityEditor;
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
        [SerializeField] [Header("Scene names")] private SceneAsset tutorialScene;
        [SerializeField] private SceneAsset jimsterburgScene;
        [SerializeField] private SceneAsset parabeneForestScene;
        [SerializeField] private SceneAsset blemburgCitadelScene;
        [SerializeField] private SceneAsset rinfretVillageScene;
        [SerializeField] private SceneAsset darkTowerScene;
        [SerializeField] private SceneAsset tulipValleyScene;
        [SerializeField] private SceneAsset morktressScene;
        [SerializeField] private SceneAsset overworldScene;
        [SerializeField] private SceneAsset mainmenuScene;
        [SerializeField] private SceneAsset gameUiScene;
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

        public int PlayerMovementRange => playerMovementRange;
        public int EnemyMovementRange => enemyMovementRange;
        public int PlayerAttackRange => playerAttackRange;
        public int EnemyAttackRange => enemyAttackRange;
        public float AttackDuration => attackDuration;
        public float MovementDuration => movementDuration;
        public int NumberOfRecruitablesOnAlternatePath => numberOfRecruitablesOnAlternatePath;
        public KeyCode SkipComputerTurnKey => skipComputerTurnKey;
        public string TutorialSceneName => tutorialScene.name;
        public string JimsterburgSceneName => jimsterburgScene.name;
        public string ParabeneForestSceneName => parabeneForestScene.name;
        public string BlemburgCitadelSceneName => blemburgCitadelScene.name;
        public string RinfretVillageSceneName => rinfretVillageScene.name;
        public string DarkTowerSceneName => darkTowerScene.name;
        public string TulipValleySceneName => tulipValleyScene.name;
        public string MorktressSceneName => morktressScene.name;
        public string OverworldSceneName => overworldScene.name;
        public string MainmenuSceneName => mainmenuScene.name;
        public string GameUiSceneName => gameUiScene.name;
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
    }
}