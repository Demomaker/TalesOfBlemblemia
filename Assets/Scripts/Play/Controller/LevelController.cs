using System.Collections;
using System.Linq;
using Harmony;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    /// <summary>
    /// Controller for each individual level. Manages the turn functionalities as well as the players' turns
    /// Authors: Mike Bédard, Jérémie Bertrand, Zacharie Lavigne, Antoine Lessard
    /// </summary>
    [Findable("LevelController")]
    public class LevelController : MonoBehaviour
    {
        private const int CREDITS_DURATION = 20;
        
        [SerializeField] private AudioClip backgroundMusic = null;
        [SerializeField] private GameObject protagonistGameObject = null;
        [SerializeField] private string customObjectiveMessage = "";
        [SerializeField] private bool completeIfPointAchieved = false;
        [SerializeField] private bool completeIfSurvivedCertainNumberOfTurns = false;
        [SerializeField] private bool completeIfCertainTargetsDefeated = false;
        [SerializeField] private Vector2Int pointToAchieve = new Vector2Int();
        [SerializeField] private Targetable[] targetsToDefeat = null;
        [SerializeField] private bool allTargetsNeedToBeDefeated;
        [SerializeField] private Targetable[] targetsToProtect = null;
        [SerializeField] private int numberOfTurnsBeforeCompletion;
        [SerializeField] private bool revertWeaponTriangle = false;
        [SerializeField] private GameObject pointingArrowPrefab = null;
        [SerializeField] private string defeatString = "Defeat ";
        [SerializeField] private string surviveString = "Survive ";
        [SerializeField] private string turnString = " turns";
        private CoroutineStarter coroutineStarter;
        private int levelTileUpdateKeeper;
        private string levelName;
        private bool levelIsEnding;
        private bool isComputerPlaying;
        private OnLevelVictory onLevelVictory;
        private OnLevelFailed onLevelFailed;
        private OnCampaignFailed onCampaignFailed;
        private UIController uiController;
        private Unit[] units;
        private UnitOwner currentPlayer;
        private int numberOfPlayerTurns;
        private GameController gameController;
        private SaveController saveController;
        private EndGameCreditsController endGameCredits;
        private OnUnitDeath onUnitDeath;
        private EnemyRangeController enemyRangeController;
        private AchievementController achievementController;
        
        protected CinematicController cinematicController;
        protected OnLevelChange onLevelChange;
        protected LevelLoader levelLoader;
        protected GameSettings gameSettings;
        
        private readonly HumanPlayer humanPlayer = new HumanPlayer();
        private readonly ComputerPlayer computerPlayer = new ComputerPlayer();

        private bool AllEnemiesDied => computerPlayer.HaveAllUnitsDied;
        private bool PointAchieved => completeIfPointAchieved && protagonistGameObject.GetComponent<Unit>()?.CurrentTile.LogicalPosition == pointToAchieve;
        private bool AllTargetsDefeated => completeIfCertainTargetsDefeated && AllTargetsToDefeatHaveBeenDefeated();
        private bool Survived => completeIfSurvivedCertainNumberOfTurns && numberOfPlayerTurns >= numberOfTurnsBeforeCompletion;
        private bool ProtagonistDied => protagonistGameObject == null || protagonistGameObject.GetComponent<Unit>().NoHealthLeft;
        private bool LevelCompleted => AllEnemiesDied || PointAchieved || AllTargetsDefeated || Survived;
        public bool PlayerUnitIsMovingOrAttacking => humanPlayer.OwnedUnits.All(unit => unit.IsMoving || unit.IsAttacking);
        private bool LevelFailed => ProtagonistDied;
        public bool LevelEnded => LevelCompleted || LevelFailed;
        public bool RevertWeaponTriangle => revertWeaponTriangle;
        public int LevelTileUpdateKeeper => levelTileUpdateKeeper;
        public virtual AudioClip BackgroundMusic => backgroundMusic;
        public virtual CinematicController CinematicController => cinematicController;
        public UnitOwner CurrentPlayer => currentPlayer;
        public HumanPlayer HumanPlayer => humanPlayer;
        public ComputerPlayer ComputerPlayer => computerPlayer;
        public bool PlayerCanPlay => PlayerUnitIsMovingOrAttacking || CinematicController.IsPlayingACinematic || CurrentPlayer is ComputerPlayer;
        public bool BattleOngoing { get; set; }
        protected virtual void Awake()
        {
            onLevelVictory = Harmony.Finder.OnLevelVictory;
            onLevelFailed = Harmony.Finder.OnLevelFailed;
            onLevelChange = Harmony.Finder.OnLevelChange;
            onCampaignFailed = Harmony.Finder.OnCampaignFailed;
            levelLoader = Harmony.Finder.LevelLoader;
            saveController = Finder.SaveController;
            gameController = Harmony.Finder.GameController;
            gameSettings = Harmony.Finder.GameSettings;
            cinematicController = GetComponent<CinematicController>();
            levelName = gameObject.scene.name;
            endGameCredits = GetComponentInChildren<EndGameCreditsController>();
            uiController = Harmony.Finder.UIController;
            if (protagonistGameObject == null) Debug.LogError("Missing ProtagonistGameObject in LevelController!");
            if (endGameCredits != null) endGameCredits.gameObject.SetActive(false);
            coroutineStarter = Harmony.Finder.CoroutineStarter;
            onUnitDeath = Harmony.Finder.OnUnitDeath;
            enemyRangeController = Harmony.Finder.EnemyRangeController;
            achievementController = Harmony.Finder.AchievementController;
        }

        protected virtual void Start()
        {
            onLevelChange.Publish(this);
            units = FindObjectsOfType<Unit>();
            GiveUnits();
            currentPlayer = humanPlayer;
            OnTurnGiven();
            ActivatePlayerUnits();
            CheckForDarkKnight();
            uiController.ModifyVictoryCondition(customObjectiveMessage);
            if (completeIfPointAchieved) CreatePointToAchievePointingArrow();
            if((levelName == gameSettings.DarkTowerSceneName || levelName == gameSettings.TulipValleySceneName) && humanPlayer.NumberOfUnits == gameSettings.NumberOfMaximumUnitsForThePlayer) 
                achievementController.UnlockAchievement(gameSettings.ReachFinalLevelWith8Players);
        }

        protected virtual void OnEnable()
        {
            onUnitDeath.Notify += computerPlayer.OnUnitDeath;
        }

        protected virtual void OnDisable()
        {
            onUnitDeath.Notify -= computerPlayer.OnUnitDeath;
        }
        
        protected virtual void Update()
        {
            if (!BattleOngoing && LevelEnded) StartCoroutine(EndLevel());
            CheckForCurrentPlayerLoss();
            CheckForCurrentPlayerEndOfTurn();
            Play();
        }
        
        private IEnumerator EndLevel()
        {
            if (levelIsEnding) yield break;
            levelIsEnding = true;
            if (LevelCompleted)
            {
                onLevelVictory.Publish(this);
                if (endGameCredits != null)
                {
                    endGameCredits.RollCredits();
                    yield return new WaitForSeconds(CREDITS_DURATION);
                    CheckForPermadeath();
                    CheckIfUnitWasRecruited();
                    CheckIfUpperPathWasTaken();
                    UpdatePlayerSave();
                    levelLoader.FadeToLevel(gameSettings.OverworldSceneName, LoadSceneMode.Additive);
                    yield break;
                }
            }
            
            if(LevelFailed) PublishFailDependingOnDifficultyLevel(gameController.DifficultyLevel);
            while (cinematicController.IsPlayingACinematic) yield return null;
            CheckForPermadeath();
            CheckIfUnitWasRecruited();
            CheckIfUpperPathWasTaken();
            UpdatePlayerSave();
            UnlockEndLevelAchievements();
            levelLoader.FadeToLevel(gameSettings.OverworldSceneName, LoadSceneMode.Additive);
        }

        private void UnlockEndLevelAchievements()
        {
            if (!humanPlayer.HasLostAUnitInCurrentLevel && levelName != gameSettings.SnowyPeaksSceneName) achievementController.UnlockAchievement(gameSettings.FinishALevelWithoutUnitLoss);
            if (levelName == gameSettings.DarkTowerSceneName) achievementController.UnlockAchievement(gameSettings.DefeatBlackKnight);
            if (levelName == gameSettings.MorktressSceneName)
            {
                achievementController.UnlockAchievement(gameSettings.CompleteCampaignOnEasy);
                if (gameController.DifficultyLevel == DifficultyLevel.Medium)
                {
                    achievementController.UnlockAchievement(gameSettings.CompleteCampaignOnMedium);
                    if(gameController.DifficultyLevel == DifficultyLevel.Hard) achievementController.UnlockAchievement(gameSettings.CompleteCampaignOnHard);
                    if(humanPlayer.NumberOfUnits == gameSettings.NumberOfMaximumUnitsForThePlayer) achievementController.UnlockAchievement(gameSettings.FinishCampaignWithoutUnitLoss);
                }
            }
        }

        private void PublishFailDependingOnDifficultyLevel(DifficultyLevel difficultyLevel)
        {
            if (difficultyLevel == DifficultyLevel.Easy)
                onLevelFailed.Publish(this);
            else
            {
                onCampaignFailed.Publish(this);
                saveController.ResetSave();
                levelLoader.FadeToLevel(gameSettings.MainmenuSceneName, LoadSceneMode.Additive);
            }
        }

        private void CreatePointToAchievePointingArrow()
        {
            var pointingArrowTransformOffset = new Vector2(0.5f, 0.5f);
            var pointingArrow = Instantiate(pointingArrowPrefab, Vector3.zero, Quaternion.identity, transform);
            pointingArrow.transform.SetParent(null);
            pointingArrow.GetComponent<PointingArrow>().SetTransformToPointPosition(new Vector3(pointToAchieve.x + pointingArrowTransformOffset.x, pointToAchieve.y + pointingArrowTransformOffset.y, 0));
        }

        private void CheckIfUpperPathWasTaken()
        {
            if (levelName == gameSettings.DarkTowerSceneName)
            {
                var characterInfos = saveController.GetCurrentSaveSelectedInfos().CharacterInfos;
                foreach (var character in characterInfos.Where(character => character.CharacterName == gameSettings.AbrahamName || character.CharacterName == gameSettings.ThomasName))
                {
                    character.CharacterStatus = false;
                }
            }
        }

        private void CheckIfUnitWasRecruited()
        {
            foreach (var unit in units)
            {
                if (!unit.IsRecruitable) continue;
                var characterInfos = saveController.GetCurrentSaveSelectedInfos().CharacterInfos;
                characterInfos.Find(info => info.CharacterName == unit.name).CharacterStatus = false;
            }
        }

        /// <summary>
        /// Check for the player units defeated during the level and mark them as defeated in the player save if the difficulty
        /// is medium or hard. Resets the save of the player if Franklem was defeated.
        /// </summary>
        private void CheckForPermadeath()
        {
            var defeatedPlayerUnits = humanPlayer.DefeatedUnits;
            if (!defeatedPlayerUnits.Any()) return;
            var characterInfos = saveController.GetCurrentSaveSelectedInfos().CharacterInfos;
            foreach (var unit in defeatedPlayerUnits.Where(unit => gameController.PermaDeath))
            {
                if (gameController.PermaDeath)
                {
                    foreach (var character in characterInfos.Where(character => character.CharacterName == unit.name))
                    {
                        character.CharacterStatus = false;
                    }
                }
            }
        }

        private bool AllTargetsToDefeatHaveBeenDefeated()
        {
            var defeatedUnitsCount = targetsToDefeat.Count(target => target == null || target.NoHealthLeft);
            return allTargetsNeedToBeDefeated ? defeatedUnitsCount == targetsToDefeat.Length : defeatedUnitsCount > 0;
        }

        private void Play()
        {
            currentPlayer.RemoveDeadUnits();
            if (isComputerPlaying || currentPlayer != computerPlayer) return;
            isComputerPlaying = true;
            coroutineStarter.StartCoroutine(computerPlayer.PlayUnits());
        }

        private void GiveUnits()
        {
            foreach (var unit in units)
            {
                if (unit.IsPlayer)
                {
                    humanPlayer.AddOwnedUnit(unit);
                    computerPlayer.AddEnemyUnit(unit);
                }
                else if (unit.IsEnemy)
                {
                    computerPlayer.AddOwnedUnit(unit);
                    humanPlayer.AddEnemyUnit(unit);
                }
            }

            foreach (var target in targetsToProtect)
                computerPlayer.AddTarget(target);
        }

        private void CheckForCurrentPlayerEndOfTurn()
        {
            if (!currentPlayer.HasNoMorePlayableUnits) return;
            currentPlayer.ResetOwnedUnitsAlpha();
            GiveTurnToNextPlayer();
            OnTurnGiven();
        }
        
        private void OnTurnGiven()
        {
            if (currentPlayer == humanPlayer)
            {
                numberOfPlayerTurns++;
                uiController.ModifyTurnCounter(numberOfPlayerTurns);
                enemyRangeController.OnPlayerTurn(computerPlayer.OwnedUnits);
            }
            else
            {
                enemyRangeController.OnComputerTurn();
            }
            uiController.ModifyTurnInfo(currentPlayer);
            currentPlayer.OnTurnGiven();
        }
        
        private void CheckForCurrentPlayerLoss()
        {
            if (currentPlayer != null && !currentPlayer.HaveAllUnitsDied) return;
            GiveTurnToNextPlayer();
        }

        private void GiveTurnToNextPlayer()
        {
            isComputerPlaying = false;
            currentPlayer = (currentPlayer != humanPlayer) ? (UnitOwner) humanPlayer : computerPlayer;
        }
        
        /// <summary>
        /// Activating only the units that are not marked as defeated in the player's save file
        /// </summary>
        private void ActivatePlayerUnits()
        {
            var characterInfos = saveController.GetCurrentSaveSelectedInfos().CharacterInfos;
            var unitsToRemove = (from gameUnit in currentPlayer.OwnedUnits
                from saveUnit in characterInfos
                where gameUnit.name == saveUnit.CharacterName && !saveUnit.CharacterStatus
                select gameUnit).ToList();

            foreach (var unit in unitsToRemove)
            {
                unit.gameObject.SetActive(false);
                currentPlayer.OwnedUnits.Remove(unit);
                computerPlayer.RemoveEnemyUnit(unit);
            }
        }
        
        /// <summary>
        /// Saves if the level was successfully completed
        /// </summary>
        private void UpdatePlayerSave()
        {
            if (!LevelCompleted) return;
            gameController.OnLevelCompleted(levelName);
            saveController.GetCurrentSaveSelectedInfos().LevelName = gameController.PreviousLevelName;
            saveController.UpdateSave(saveController.SaveSelected);
        }

        public void IncrementTileUpdate()
        {
            levelTileUpdateKeeper++;
        }
        
        private void CheckForDarkKnight()
        {
            if (levelName == gameSettings.MorktressSceneName && gameController.PreviousLevelName == gameSettings.DarkTowerSceneName)
            {
                var unit = computerPlayer.OwnedUnits.Find(info => info.name == gameSettings.DarkKnightName);
                if (unit != null) unit.gameObject.SetActive(false);
            }
        }
    }
}