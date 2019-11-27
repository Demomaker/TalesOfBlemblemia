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
    /// Controller for each individual level. Manages the turn functionalities as well as the players' turns
    /// Authors: Mike Bédard, Jérémie Bertrand, Zacharie Lavigne, Antoine Lessard
    /// </summary>
    [Findable("LevelController")]
    public class LevelController : MonoBehaviour
    {
        #region Constants
        private const string PROTAGONIST_NAME = "Franklem";
        private const string REACH_TARGET_VICTORY_CONDITION_TEXT = "Reach the target!";
        private const string DEFEAT_ALL_ENEMIES_VICTORY_CONDITION_TEXT = "Defeat all the enemies!";
        private const int CREDITS_DURATION = 20;
        #endregion Constants
        #region Serialized Fields
        [SerializeField] private AudioClip backgroundMusic;
        [SerializeField] private bool doNotEnd;
        [SerializeField] private string customObjectiveMessage = null;
        [SerializeField] private bool completeIfPointAchieved = false;
        [SerializeField] private bool completeIfSurvivedCertainNumberOfTurns = false;
        [SerializeField] private bool completeIfCertainTargetsDefeated = false;
        [SerializeField] private Vector2Int pointToAchieve = new Vector2Int();
        [SerializeField] private Targetable[] targetsToDefeat = null;
        [SerializeField] private bool allTargetsNeedToBeDefeated;
        [SerializeField] private Targetable[] targetsToProtect = null;
        [SerializeField] private int numberOfTurnsBeforeCompletion;
        [SerializeField] private bool revertWeaponTriangle = false;
        [SerializeField] private UnityEngine.Object pointingArrowPrefab = null;
        #endregion Serialized Fields
        #region ReadOnly Fields
        private readonly List<UnitOwner> players = new List<UnitOwner>();
        #endregion ReadOnly Fields
        #region Other Fields
        private CinematicController cinematicController;
        private int levelTileUpdateKeeper;
        private string levelName;
        private bool levelIsEnding;
        private bool isComputerPlaying;
        private bool skipLevel = false;
        private OnLevelVictory onLevelVictory;
        private OnLevelFailed onLevelFailed;
        private GameObject dialogueUi;
        private OnLevelChange onLevelChange;
        private OnCampaignFailed onCampaignFailed;
        private UIController uiController;
        private LevelLoader levelLoader;
        private Unit[] units;
        private UnitOwner currentPlayer;
        private int numberOfPlayerTurns = 0;
        private GameSettings gameSettings;
        private GameController gameController;
        private SaveController saveController;
        private EndGameCreditsController endGameCredits;
        #endregion Other Fields
        #region Accessors
        private bool AllEnemiesDied => ComputerPlayer.Instance.HaveAllUnitsDied();
        private bool PointAchieved => completeIfPointAchieved &&
                                      (GameObject.Find(PROTAGONIST_NAME) != null) &&
                                      (GameObject.Find(PROTAGONIST_NAME).GetComponent<Unit>() != null) &&
                                      (GameObject.Find(PROTAGONIST_NAME).GetComponent<Unit>().CurrentTile != null) &&
                                      GameObject.Find(PROTAGONIST_NAME).GetComponent<Unit>().CurrentTile.LogicalPosition == pointToAchieve;
        private bool AllTargetsDefeated => completeIfCertainTargetsDefeated && AllTargetsToDefeatHaveBeenDefeated();
        private bool Survived => completeIfSurvivedCertainNumberOfTurns && numberOfPlayerTurns >= numberOfTurnsBeforeCompletion;
        private bool ProtagonistDied => (GameObject.Find(PROTAGONIST_NAME) == null ||
                                         GameObject.Find(PROTAGONIST_NAME).GetComponent<Unit>().NoHealthLeft);
        private bool LevelCompleted => 
            skipLevel || 
            AllEnemiesDied ||
            PointAchieved || 
            AllTargetsDefeated || 
            Survived;
        private bool LevelFailed => ProtagonistDied;
        private bool LevelEnded => LevelCompleted || LevelFailed;
        public bool RevertWeaponTriangle => revertWeaponTriangle;
        
        public int LevelTileUpdateKeeper => levelTileUpdateKeeper;

        public AudioClip BackgroundMusic => backgroundMusic;
        public CinematicController CinematicController => cinematicController;
        public UnitOwner CurrentPlayer => currentPlayer;

        public bool PlayerUnitIsMovingOrAttacking
        {
            get
            {
                var playerUnitIsMovingOrAttacking = false;
                foreach (var unit in players[0].OwnedUnits.Where(unit => unit.IsMoving || unit.IsAttacking))
                {
                    playerUnitIsMovingOrAttacking = true;
                }
                return playerUnitIsMovingOrAttacking;
            }
        }

        public List<Unit> EnemyUnits => players[1].OwnedUnits;

        #endregion Accessors
        #region Unity Event Functions
        private void Awake()
        {
            InitializeEvents();
            levelLoader = Harmony.Finder.LevelLoader;
            ResetVariables();
            saveController = Finder.SaveController;
            gameController = Finder.GameController;
            gameSettings = Harmony.Finder.GameSettings;
            dialogueUi = GameObject.FindWithTag(Tags.DIALOGUE_UI_TAG);
            cinematicController = GetComponent<CinematicController>();
            levelName = gameObject.scene.name;
            endGameCredits = GetComponentInChildren<EndGameCreditsController>();
            if (endGameCredits != null)
                endGameCredits.gameObject.SetActive(false);
        }
        
        private void Start()
        {
            uiController = Harmony.Finder.UIController;
            onLevelChange.Publish(this);
            InitializePlayersAndUnits();
            currentPlayer = players[0];
            OnTurnGiven();
            if (dialogueUi != null)
            {
                dialogueUi.SetActive(true);
            }
            
            ActivatePlayerUnits();

            CheckForDarkKnight();
            
            PrepareVictoryConditionForUI();
        }
        
        protected void Update()
        {
            //TODO enlever ca
            if(Input.GetKeyDown(gameSettings.SkipLevelKey))
            {
                skipLevel = true;
            }
            //TODO enlever doNotEnd du projet en entier, right?
            if (!doNotEnd && LevelEnded)
            {
                ResetUnitsAlpha();
                StartCoroutine(EndLevel());
            }

            if (currentPlayer == null) throw new NullReferenceException("Current player is null!");
            
            //TODO enlever ca avant la release
            if (!cinematicController.IsPlayingACinematic)
            {
                CheckForComputerTurnSkip();
                CheckForPlayerTurnSkip();
            }
            
            CheckForCurrentPlayerLoss();
            CheckForCurrentPlayerEndOfTurn();
            Play(currentPlayer);
        }
        #endregion
        #region Event Channel Handling
        private void InitializeEvents()
        {
            onLevelVictory = Harmony.Finder.OnLevelVictory;
            onLevelFailed = Harmony.Finder.OnLevelFailed;
            onLevelChange = Harmony.Finder.OnLevelChange;
            onCampaignFailed = Harmony.Finder.OnCampaignFailed;
        }
        
        private void PublishFailDependingOnDifficultyLevel(DifficultyLevel difficultyLevel)
        {
            if (difficultyLevel == DifficultyLevel.Easy)
            {
                onLevelFailed.Publish(this);
            }
            else
            {
                onCampaignFailed.Publish(this);
            }
        }
        #endregion
        #region UI-related Functions
        private void PrepareVictoryConditionForUI()
        {
            if (!string.IsNullOrEmpty(customObjectiveMessage))
            {
                uiController.ModifyVictoryCondition(customObjectiveMessage);
                return;
            }
            if (completeIfPointAchieved)
            {
                uiController.ModifyVictoryCondition(REACH_TARGET_VICTORY_CONDITION_TEXT);
                CreatePointToAchievePointingArrow();
            }
            else if (completeIfCertainTargetsDefeated)
            {
                uiController.ModifyVictoryCondition("Defeat " + GetStringOfTargetsToDefeat());
            }
            else if (completeIfSurvivedCertainNumberOfTurns)
            {
                uiController.ModifyVictoryCondition("Survive " + numberOfTurnsBeforeCompletion + " turns");
            }
        }

        private void CreatePointToAchievePointingArrow()
        {
            var pointingArrowTransformOffset = new Vector2(0.5f, 0.5f);
            GameObject pointingArrow = (GameObject)GameObject.Instantiate(pointingArrowPrefab, Vector3.zero, Quaternion.identity, transform);
            pointingArrow.transform.SetParent(null);
            pointingArrow.GetComponent<PointingArrow>().SetTransformToPointPosition(new Vector3(pointToAchieve.x + pointingArrowTransformOffset.x, pointToAchieve.y + pointingArrowTransformOffset.y, 0));
        }

        private string GetStringOfTargetsToDefeat()
        {
            var additionalWord = allTargetsNeedToBeDefeated ? "and" : "or";
            var stringBetweenTargets = ", ";
            var result = "";
            for (var targetIndex = 0; targetIndex < targetsToDefeat.Length; targetIndex++)
            {
                var target = targetsToDefeat[targetIndex];
                if (targetIndex == targetsToDefeat.Length - 1) stringBetweenTargets = "";
                if (targetIndex == targetsToDefeat.Length - 2) stringBetweenTargets = " " + additionalWord + " ";
                result += (target.name + stringBetweenTargets);
            }
            return result;
        }
        #endregion
        #region Level Controlling Functions
        private void InitializePlayersAndUnits()
        {
            var player1 = HumanPlayer.Instance;
            var player2 = ComputerPlayer.Instance;

            player1.OwnedUnits.Clear();
            player1.DefeatedUnits.Clear();
            player2.OwnedUnits.Clear();
            
            units = FindObjectsOfType<Unit>();

            GiveUnits(units, player1, player2);

            player1.OnNewLevel();
            
            players.Add(player1);
            players.Add(player2);
        }
        private IEnumerator EndLevel()
        {
            if (levelIsEnding) yield break;
            levelIsEnding = true;
            if(LevelCompleted) onLevelVictory.Publish(this);
            if(LevelFailed) PublishFailDependingOnDifficultyLevel(gameController.DifficultyLevel);
            while (cinematicController.IsPlayingACinematic)
                yield return null;
            if (endGameCredits != null)
            {
                endGameCredits.RollCredits();
                yield return new WaitForSeconds(CREDITS_DURATION);
            }
            
            CheckForPermadeath();

            CheckIfUnitWasRecruited();

            CheckIfUpperPathWasTaken();
            
            UpdatePlayerSave();

            levelLoader.FadeToLevel(gameSettings.OverworldSceneName, LoadSceneMode.Additive);
        }

        private void CheckIfUpperPathWasTaken()
        {
            if (levelName == gameSettings.DarkTowerSceneName)
            {
                var characterInfos = saveController.GetCurrentSaveSelectedInfos().CharacterInfos;
                foreach (var character in characterInfos.Where(character =>
                    character.CharacterName == gameSettings.AbrahamName || character.CharacterName == gameSettings.ThomasName))
                {
                    character.CharacterStatus = false;
                }
            }
        }

        private void CheckIfUnitWasRecruited()
        {
            foreach (var unit in units)
            {
                if (unit.IsRecruitable)
                {
                    var characterInfos = saveController.GetCurrentSaveSelectedInfos().CharacterInfos;

                    characterInfos.Find(info => info.CharacterName == unit.name).CharacterStatus = false;
                    break;
                }
            }
        }

        /// <summary>
        /// Check for the player units defeated during the level and mark them as defeated in the player save if the difficulty
        /// is medium or hard. Resets the save of the player if Franklem was defeated.
        /// </summary>
        private void CheckForPermadeath()
        {
            var defeatedPlayerUnits = HumanPlayer.Instance.DefeatedUnits;

            if (!defeatedPlayerUnits.Any()) return;
            var characterInfos = saveController.GetCurrentSaveSelectedInfos().CharacterInfos;

            foreach (var unit in defeatedPlayerUnits)
            {
                if (gameController.PermaDeath)
                {
                    foreach (var character in characterInfos.Where(character => character.CharacterName == unit.name))
                    {
                        if (character.CharacterName == gameSettings.FranklemName)
                        {
                            saveController.ResetSave();
                            break;
                        }
                        character.CharacterStatus = false;
                    }
                }
            }
        }

        private void OnTurnGiven()
        {
            if (currentPlayer is HumanPlayer)
            {
                numberOfPlayerTurns++;
                Harmony.Finder.EnemyRangeController.OnPlayerTurn(players[1].OwnedUnits);
            }
            else
            {
                Harmony.Finder.EnemyRangeController.OnComputerTurn();
            }
            uiController.ModifyTurnInfo(currentPlayer);
            uiController.ModifyTurnCounter(numberOfPlayerTurns);
            currentPlayer.OnTurnGiven();
        }

        private bool AllTargetsToDefeatHaveBeenDefeated()
        {
            return allTargetsNeedToBeDefeated
                ? targetsToDefeat.Count(target => target == null || target.NoHealthLeft) == targetsToDefeat.Length
                : targetsToDefeat.Count(target => target == null || target.NoHealthLeft) > 0;
        }

        private void Play(UnitOwner unitOwner)
        {
            unitOwner.RemoveDeadUnits();
            if (isComputerPlaying || !(unitOwner is ComputerPlayer currentComputerPlayer)) return;
            isComputerPlaying = true;
            StartCoroutine(currentComputerPlayer.PlayUnits());
        }

        private void CheckForPlayerTurnSkip()
        {
            if (!Input.GetKeyDown(gameSettings.SkipComputerTurnKey) || !(currentPlayer is HumanPlayer)) return;
            isComputerPlaying = false;
            currentPlayer = players.Find(player => player is ComputerPlayer);
            OnTurnGiven();
        }

        private void CheckForComputerTurnSkip()
        {
            if (!Input.GetKeyDown(gameSettings.SkipComputerTurnKey) || !(currentPlayer is ComputerPlayer)) return;
            isComputerPlaying = false;
            currentPlayer = players.Find(player => player is HumanPlayer);
            OnTurnGiven();
        }
        
        private void GiveUnits(   
            IEnumerable<Unit> units,
            HumanPlayer player, 
            ComputerPlayer aiPlayer)
        {
            foreach (var unit in units)
            {
                if (unit.IsPlayer)
                {
                    player.AddOwnedUnit(unit);
                    aiPlayer.AddEnemyUnit(unit);
                }
                else if (unit.IsEnemy)
                {
                    aiPlayer.AddOwnedUnit(unit);
                    player.AddEnemyUnit(unit);
                }
            }

            foreach (var target in targetsToProtect)
            {
                aiPlayer.AddTarget(target);
            }
        }

        private void CheckForCurrentPlayerEndOfTurn()
        {
            if (!currentPlayer.HasNoMorePlayableUnits) return;
            ResetUnitsAlpha();
            GiveTurnToNextPlayer();
            OnTurnGiven();
        }


        private void CheckForCurrentPlayerLoss()
        {
            if (currentPlayer != null && !currentPlayer.HaveAllUnitsDied()) return;
            currentPlayer.Lose();
            GiveTurnToNextPlayer();
        }

        private void GiveTurnToNextPlayer()
        {
            isComputerPlaying = false;
            //currentPlayer.MakeOwnedUnitsUnplayable();
            var nextPlayerIndex = (players.IndexOf(currentPlayer) + 1) % 2;
        
            if (players.ElementAt(nextPlayerIndex) != null)
                currentPlayer = players.ElementAt(nextPlayerIndex);
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
                players.Find(owner => owner != currentPlayer).RemoveEnemyUnit(unit);
            }
        }
        #endregion
        #region Other Functions
        private void ResetVariables()
        {
            levelIsEnding = false;
            units = null;
            levelTileUpdateKeeper = 0;
            levelName = "";
            numberOfPlayerTurns = 0;
            players.Clear();
            HumanPlayer.Instance.OwnedUnits.Clear();
            HumanPlayer.Instance.DefeatedUnits.Clear();
            ComputerPlayer.Instance.OwnedUnits.Clear();
        }
        /// <summary>
        /// Saves if the level was successfully completed
        /// </summary>
        private void UpdatePlayerSave()
        {
            //If the level was successfully completed, mark it as completed
            if (!LevelCompleted) return;
            gameController.OnLevelCompleted(levelName);

            saveController.GetCurrentSaveSelectedInfos().LevelName = gameController.PreviousLevelName;

            saveController.UpdateSave(saveController.SaveSelected);
        }
        
        private void ResetUnitsAlpha()
        {
            foreach (var unit in currentPlayer.OwnedUnits)
            {
                SpriteRenderer[] spriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
                foreach (var spriteRenderer in spriteRenderers)
                {
                    spriteRenderer.color = gameSettings.OpaqueAlpha;
                }
            }
        }
        
        public void IncrementTileUpdate()
        {
            levelTileUpdateKeeper++;
        }
        
        private void CheckForDarkKnight()
        {
            if (levelName == gameSettings.MorktressSceneName && gameController.PreviousLevelName == gameSettings.DarkTowerSceneName)
            {
                if (ComputerPlayer.Instance.OwnedUnits.Find(info => info.name == gameSettings.DarkKnightName) != null)
                    ComputerPlayer.Instance.OwnedUnits.Find(info => info.name == gameSettings.DarkKnightName).gameObject.SetActive(false);
            }
        }
        #endregion
    }
}