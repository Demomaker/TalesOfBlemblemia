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
        
        private const string PROTAGONIST_NAME = "Franklem";
        private const string REACH_TARGET_VICTORY_CONDITION_TEXT = "Reach the target!";
        private const string DEFEAT_ALL_ENEMIES_VICTORY_CONDITION_TEXT = "Defeat all the enemies!";
        private const string PLAYER_TURN_INFO = "Player";
        private const string ENEMY_TURN_INFO = "Enemy";
        
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
        
        private readonly List<UnitOwner> players = new List<UnitOwner>();
        
        private CinematicController cinematicController;
        private int levelTileUpdateKeeper;
        private string levelName;
        private bool levelIsEnding;
        private bool isComputerPlaying;
        private OnLevelVictory onLevelVictory;
        private GameObject dialogueUi;
        private OnLevelChange onLevelChange;
        private UIController uiController;
        private LevelLoader levelLoader;

        private Unit[] units;
        private UnitOwner currentPlayer;
        private int numberOfPlayerTurns = 0;
        private GameSettings gameSettings;
        private GameController gameController;
        private SaveController saveController;
        private bool allEnemiesDied => ComputerPlayer.Instance.HaveAllUnitsDied();
        private bool pointAchieved => completeIfPointAchieved && 
                                      (GameObject.Find(PROTAGONIST_NAME) != null) &&
                                      (GameObject.Find(PROTAGONIST_NAME).GetComponent<Unit>() != null) &&
                                      (GameObject.Find(PROTAGONIST_NAME).GetComponent<Unit>().CurrentTile != null) &&
                                      GameObject.Find(PROTAGONIST_NAME).GetComponent<Unit>().CurrentTile.LogicalPosition == pointToAchieve;
        private bool allTargetsDefeated => completeIfCertainTargetsDefeated && AllTargetsToDefeatHaveBeenDefeated();
        private bool survived => completeIfSurvivedCertainNumberOfTurns && numberOfPlayerTurns >= numberOfTurnsBeforeCompletion;

        private bool protagonistDied => (GameObject.Find(PROTAGONIST_NAME) == null ||
                                         GameObject.Find(PROTAGONIST_NAME).GetComponent<Unit>().NoHealthLeft);
        private bool skipLevel = false;
        private bool levelCompleted => 
            skipLevel || 
            allEnemiesDied ||
            pointAchieved || 
            allTargetsDefeated || 
            survived;
        private bool levelFailed => protagonistDied;
        private bool levelEnded => levelCompleted || levelFailed;
        public bool RevertWeaponTriangle => revertWeaponTriangle;
        public int LevelTileUpdateKeeper => levelTileUpdateKeeper;

        public AudioClip BackgroundMusic => backgroundMusic;
        public CinematicController CinematicController => cinematicController;

        public bool PlayerUnitIsMovingOrAttacking
        {
            get
            {
                bool playerUnitIsMovingOrAttacking = false;
                foreach (var unit in players[0].OwnedUnits)
                {
                    if (unit.IsMoving || unit.IsAttacking)
                        playerUnitIsMovingOrAttacking = true;
                }
                return playerUnitIsMovingOrAttacking;
            }
        }

        private void Awake()
        {
            levelLoader = Harmony.Finder.LevelLoader;
            ResetVariables();
            saveController = Finder.SaveController;
            gameController = Finder.GameController;
            gameSettings = Harmony.Finder.GameSettings;
            dialogueUi = GameObject.FindWithTag("DialogueUi");
            cinematicController = GetComponent<CinematicController>();
            onLevelVictory = Harmony.Finder.OnLevelVictory;
            onLevelChange = Harmony.Finder.OnLevelChange;
            levelName = gameObject.scene.name;
        }

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

            PrepareVictoryConditionForUI();
        }

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

        protected void Update()
        {
            if(Input.GetKeyDown(KeyCode.O))
            {
                skipLevel = true;
            }
            
            if (!doNotEnd && levelEnded)
            {
                ResetUnitsAlpha();
                StartCoroutine(EndLevel());
            }

            if (currentPlayer == null) throw new NullReferenceException("Current player is null!");
            
            //TODO enlever ca avant la release
            CheckForComputerTurnSkip();
            CheckForPlayerTurnSkip();
            CheckForCurrentPlayerWin();
            CheckForCurrentPlayerLoss();
            CheckForCurrentPlayerEndOfTurn();
            Play(currentPlayer);
        }

        private IEnumerator EndLevel()
        {
            if (levelIsEnding) yield break;
            levelIsEnding = true;
            if(levelCompleted) onLevelVictory.Publish(this);
            while (cinematicController.IsPlayingACinematic)
            {
                yield return null;
            }
            
            CheckForPermadeath();

            UpdatePlayerSave();

            levelLoader.FadeToLevel(gameSettings.OverworldSceneName, LoadSceneMode.Additive);
        }

        /// <summary>
        /// Saves if the level was successfully completed
        /// </summary>
        private void UpdatePlayerSave()
        {
            //If the level was successfully completed, mark it as completed
            if (!levelCompleted) return;
            gameController.OnLevelCompleted(levelName);

            saveController.GetCurrentSaveSelectedInfos().LevelName = gameController.PreviousLevelName;

            saveController.UpdateSave(saveController.SaveSelected);
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
                if (unit.name == gameSettings.FranklemName)
                {
                    saveController.ResetSave();
                    break;
                }

                if (gameController.PermaDeath)
                {
                    foreach (var character in characterInfos.Where(character => character.CharacterName == unit.name))
                    {
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
                uiController.ModifyTurnInfo(PLAYER_TURN_INFO);
            }
            else
            {
                uiController.ModifyTurnInfo(ENEMY_TURN_INFO);
            }
            uiController.ModifyTurnCounter(numberOfPlayerTurns);
            
            currentPlayer.OnTurnGiven();
        }

        private bool AllTargetsToDefeatHaveBeenDefeated()
        {
            return allTargetsNeedToBeDefeated
                ? targetsToDefeat.Count(target => target == null || target.NoHealthLeft) == targetsToDefeat.Length
                : targetsToDefeat.Count(target => target == null || target.NoHealthLeft) > 0;
        }

        private bool TargetToProtectHasDied()
        {
            return targetsToProtect.Any(target => target != null && target.NoHealthLeft);
        }

        private void Play(UnitOwner unitOwner)
        {
            unitOwner.RemoveDeadUnits();
            if (!isComputerPlaying && unitOwner is ComputerPlayer)
            {
                isComputerPlaying = true;
                var currentComputerPlayer = unitOwner as ComputerPlayer;
                StartCoroutine(currentComputerPlayer.PlayUnits());
            }
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
        
        private void InitializePlayersAndUnits()
        {
            var player1 = HumanPlayer.Instance;
            var player2 = ComputerPlayer.Instance;

            player1.OwnedUnits.Clear();
            player1.DefeatedUnits.Clear();
            player2.OwnedUnits.Clear();
            
            units = FindObjectsOfType<Unit>();

            GiveUnits(units, player1, player2);

            player1.UpdateNumberOfStartingOwnedUnits();
            player1.OnNewLevel();
            player2.OnNewLevel();
            
            players.Add(player1);
            players.Add(player2);
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
            if (currentPlayer.HasNoMorePlayableUnits)
            {
                ResetUnitsAlpha();
                GiveTurnToNextPlayer();
                OnTurnGiven();
            }
        }

        private void ResetUnitsAlpha()
        {
            foreach (var unit in currentPlayer.OwnedUnits)
            {
                unit.gameObject.GetComponent<SpriteRenderer>().color = gameSettings.OpaqueAlpha;
            }
        }

        private void CheckForCurrentPlayerWin()
        {
            if (HasWon(currentPlayer))
            {
                currentPlayer.Win();
            }
        }

        private void CheckForCurrentPlayerLoss()
        {
            if (!currentPlayer.HaveAllUnitsDied()) return;
            currentPlayer.Lose();
            var playerWhoLost = currentPlayer;
            GiveTurnToNextPlayer();
        }

        private bool HasWon(UnitOwner unitOwner)
        {
            return players.Contains(unitOwner) && players.Count <= 1;
        }

        private void GiveTurnToNextPlayer()
        {
            isComputerPlaying = false;
            currentPlayer.MakeOwnedUnitsUnplayable();
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

            foreach (
                var gameUnit in 
                from gameUnit in currentPlayer.OwnedUnits
                from saveUnit in characterInfos
                where gameUnit.name == saveUnit.CharacterName && !saveUnit.CharacterStatus
                select gameUnit)
            {
                gameUnit.gameObject.SetActive(false);
            }
        }
        
        public void IncrementTileUpdate()
        {
            levelTileUpdateKeeper++;
        }
    }
}
