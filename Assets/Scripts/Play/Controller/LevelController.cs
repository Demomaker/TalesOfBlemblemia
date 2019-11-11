using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Linq;
using Game;
using Harmony;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Finder = Harmony.Finder;

namespace Game
{
    /// <summary>
    /// Controller for each individual level. Manages the turn functionalities as well as the players' turns
    /// Authors: Mike Bédard, Jérémie Bertrand, Zacharie Lavigne
    /// </summary>
    [Findable("LevelController")]
    public class LevelController : MonoBehaviour
    {
        
        private const string PROTAGONIST_NAME = "Franklem";
        
        [SerializeField] private string levelName;
        [SerializeField] private AudioClip backgroundMusic;
        [SerializeField] private LevelBackgroundMusicType backgroundMusicOption;
        [SerializeField] private DialogueTrigger dialogueTriggerStartFranklem = null;
        [SerializeField] private bool doNotEnd;
        [SerializeField] private bool completeIfAllEnemiesDefeated = false;
        [SerializeField] private bool completeIfPointAchieved = false;
        [SerializeField] private bool completeIfSurvivedCertainNumberOfTurns = false;
        [SerializeField] private bool completeIfCertainEnemyDefeated = false;
        [SerializeField] private bool defeatIfNotCompleteLevelInCertainAmountOfTurns = false;
        [SerializeField] private bool defeatIfProtectedIsKilled = false;
        [SerializeField] private bool defeatIfAllPlayerUnitsDied = false;
        [SerializeField] private Vector2Int pointToAchieve = new Vector2Int();
        [SerializeField] private Unit enemyToDefeat = null;
        [SerializeField] private Targetable[] targetsToProtect = null;
        [SerializeField] private int numberOfTurnsBeforeDefeat = 0;
        [SerializeField] private int numberOfTurnsBeforeCompletion = 0;
        [SerializeField] private bool revertWeaponTriangle = false;
        
        private int levelTileUpdateKeeper = 0;
        

        private const string REACH_TARGET_VICTORY_CONDITION_TEXT = "Reach the target!";
        private const string DEFEAT_ALL_ENEMIES_VICTORY_CONDITION_TEXT = "Defeat all the enemies!";



        private CinematicController cinematicController;
        public CinematicController CinematicController => cinematicController;

        private bool levelCompleted = false;
        private bool levelFailed = false;
        private bool levelEnded = false;
        private bool levelIsEnding = false;
        private bool isComputerPlaying;
        private OnLevelVictory onLevelVictory;
        private GameObject dialogueUi;
        private OnLevelChange onLevelChange;

        private Unit[] units = null;
        private UnitOwner currentPlayer;
        private readonly List<UnitOwner> players = new List<UnitOwner>();
        private int numberOfPlayerTurns = 0;
        public bool RevertWeaponTriangle => revertWeaponTriangle;
        public int LevelTileUpdateKeeper => levelTileUpdateKeeper;

        public AudioClip BackgroundMusic => backgroundMusic;

        private void Awake()
        {
            dialogueUi = GameObject.FindWithTag("DialogueUi");
            cinematicController = GetComponent<CinematicController>();
            onLevelVictory = Harmony.Finder.OnLevelVictory;
            onLevelChange = Harmony.Finder.OnLevelChange;
        }

        private void Start()
        {
            onLevelChange.Publish(this);
            players.Clear();
            InitializePlayersAndUnits();
            currentPlayer = players[0];
            OnTurnGiven();
            if (dialogueUi != null && dialogueTriggerStartFranklem != null)
            {
                dialogueUi.SetActive(true);
                dialogueTriggerStartFranklem.TriggerDialogue();
            }
            
            ActivatePlayerUnits();

            PrepareVictoryConditionForUI();
        }

        private void PrepareVictoryConditionForUI()
        {
            if (completeIfPointAchieved)
            {
                Harmony.Finder.UIController.ModifyVictoryCondition(REACH_TARGET_VICTORY_CONDITION_TEXT);
            }
            else if (completeIfAllEnemiesDefeated)
            {
                Harmony.Finder.UIController.ModifyVictoryCondition(DEFEAT_ALL_ENEMIES_VICTORY_CONDITION_TEXT);
            }
            else if (completeIfCertainEnemyDefeated)
            {
                Harmony.Finder.UIController.ModifyVictoryCondition("Defeat " + enemyToDefeat);
            }
            else if (completeIfSurvivedCertainNumberOfTurns)
            {
                Harmony.Finder.UIController.ModifyVictoryCondition("Survive " + numberOfTurnsBeforeCompletion + " turns");
            }
        }

        protected void Update()
        {
            if(!doNotEnd) CheckIfLevelEnded();
            
            if (Input.GetKeyDown(KeyCode.O))
            {
                levelCompleted = true;
                levelEnded = true;
            }

            if (levelEnded)
            {
                if (levelCompleted)
                {
                    onLevelVictory.Publish(this);
                }
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
            
            while (cinematicController.IsPlayingACinematic)
            {
                yield return null;
            }
            
            CheckForPermadeath();

            UpdatePlayerSave();
            
            //Finder.GameController.LoadLevel(Constants.OVERWORLD_SCENE_NAME);
        }

        /// <summary>
        /// Saves if the level was successfully completed
        /// </summary>
        private void UpdatePlayerSave()
        {
            //If the level was successfully completed, mark it as completed
            if (levelCompleted)
            {
                Finder.GameController.OnLevelCompleted(levelName);

                var levels = Finder.GameController.Levels;
                foreach (var level in levels)
                {
                    if (level.PreviousLevel == levelName)
                    {
                        Finder.SaveController.GetCurrentSaveSelectedInfos().LevelName = level.LevelName;
                        break;
                    }
                }

                Finder.SaveController.UpdateSave(Finder.SaveController.SaveSelected);
            }
        }

        /// <summary>
        /// Check for the player units defeated during the level and mark them as defeated in the player save if the difficulty
        /// is medium or hard. Resets the save of the player if Franklem was defeated.
        /// </summary>
        private void CheckForPermadeath()
        {
            var defeatedPlayerUnits = HumanPlayer.Instance.DefeatedUnits;

            if (defeatedPlayerUnits.Any())
            {
                var characterInfos = Finder.SaveController.GetCurrentSaveSelectedInfos().CharacterInfos;

                foreach (var unit in defeatedPlayerUnits)
                {
                    if (unit.name == Constants.FRANKLEM_NAME)
                    {
                        Finder.SaveController.ResetSave();
                        break;
                    }

                    if (Finder.GameController.DifficultyLevel != DifficultyLevel.Easy && levelCompleted)
                    {
                        foreach (var character in characterInfos)
                        {
                            if (character.CharacterName == unit.name)
                            {
                                character.CharacterStatus = false;
                            }
                        }
                    }
                }
            }
        }

        private void OnTurnGiven()
        {
            if(currentPlayer is HumanPlayer) numberOfPlayerTurns++;
            Harmony.Finder.UIController.ModifyTurnCounter(numberOfPlayerTurns);
            currentPlayer.OnTurnGiven();
        }

        private void CheckIfLevelEnded()
        {
            CheckIfLevelCompleted();
            CheckIfLevelFailed();
            if (levelFailed || levelCompleted) levelEnded = true;
        }

        private void CheckIfLevelCompleted()
        {
            bool firstConditionAchieved = true;
            bool secondConditionAchieved = true;
            bool thirdConditionAchieved = true;
            bool fourthConditionAchieved = true;
            if (completeIfAllEnemiesDefeated)
            {
                if(!ComputerPlayer.Instance.HaveAllUnitsDied()) firstConditionAchieved = false;
            }
            if (completeIfPointAchieved)
            {
                if ((GameObject.Find(PROTAGONIST_NAME) == null) 
                || (GameObject.Find(PROTAGONIST_NAME).GetComponent<Unit>() == null) 
                || (GameObject.Find(PROTAGONIST_NAME).GetComponent<Unit>().CurrentTile == null) 
                || !(GameObject.Find(PROTAGONIST_NAME).GetComponent<Unit>().CurrentTile.LogicalPosition == pointToAchieve)) secondConditionAchieved = false;
            }
            if (completeIfCertainEnemyDefeated)
            {
                if (!(enemyToDefeat == null || enemyToDefeat.NoHealthLeft)) thirdConditionAchieved = false;
            }
            if (completeIfSurvivedCertainNumberOfTurns)
            {
                if (numberOfPlayerTurns < numberOfTurnsBeforeCompletion) fourthConditionAchieved = false;
            }

            levelCompleted = firstConditionAchieved && secondConditionAchieved && thirdConditionAchieved && fourthConditionAchieved;
            if (levelCompleted) onLevelVictory.Publish(this);
        }

        private void CheckIfLevelFailed()
        {
            levelFailed =
                defeatIfNotCompleteLevelInCertainAmountOfTurns && (numberOfPlayerTurns >= numberOfTurnsBeforeDefeat) ||
                (defeatIfProtectedIsKilled && TargetToProtectHasDied()) ||
                (defeatIfAllPlayerUnitsDied &&
                 HumanPlayer.Instance.HaveAllUnitsDied()
                ) || (GameObject.Find(PROTAGONIST_NAME) == null || GameObject.Find(PROTAGONIST_NAME).GetComponent<Unit>().NoHealthLeft);
        }

        private bool TargetToProtectHasDied()
        {
            foreach (var target in targetsToProtect)
            {
                if (target != null && target.NoHealthLeft) return true;
            }
            return false;
        }

        private void Play(UnitOwner unitOwner)
        {
            unitOwner.CheckUnitDeaths();
            if (!isComputerPlaying && unitOwner is ComputerPlayer)
            {
                isComputerPlaying = true;
                var currentComputerPlayer = unitOwner as ComputerPlayer;
                StartCoroutine(currentComputerPlayer.PlayUnits());
            }
        }

        private void CheckForPlayerTurnSkip()
        {
            if (Input.GetKeyDown(Constants.SKIP_COMPUTER_TURN_KEY) && currentPlayer is HumanPlayer)
            {
                isComputerPlaying = false;
                currentPlayer = players.Find(player => player is ComputerPlayer);
                OnTurnGiven();
            }
        }

        private void CheckForComputerTurnSkip()
        {
            if (Input.GetKeyDown(Constants.SKIP_COMPUTER_TURN_KEY) && currentPlayer is ComputerPlayer)
            {
                isComputerPlaying = false;
                currentPlayer = players.Find(player => player is HumanPlayer);
                OnTurnGiven();
            }
        }
        
        private void InitializePlayersAndUnits()
        {
            var player1 = HumanPlayer.Instance;
            var player2 = ComputerPlayer.Instance;

            units = FindObjectsOfType<Unit>();

            GiveUnits(units, player1, player2);

            player1.UpdateNumberOfStartingOwnedUnits();
            player1.OnNewLevel();
            player2.OnNewLevel();
            
            players.Add(player1);
            players.Add(player2);
        }

        private void GiveUnits(Unit[] units, HumanPlayer player, ComputerPlayer aiPlayer)
        {
            for (int i = 0; i < units.Length; i++)
            {
                if (units[i].IsPlayer)
                {
                    player.AddOwnedUnit(units[i]);
                    aiPlayer.AddEnemyUnit(units[i]);
                }
                else if (units[i].IsEnemy)
                {
                    aiPlayer.AddOwnedUnit(units[i]);
                    player.AddEnemyUnit(units[i]);
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
                GiveTurnToNextPlayer();
                OnTurnGiven();
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
            if (currentPlayer.HaveAllUnitsDied())
            {
                currentPlayer.Lose();
                UnitOwner playerWhoLost = currentPlayer;
                GiveTurnToNextPlayer();
            }
        }

        private bool HasWon(UnitOwner unitOwner)
        {
            return players.Contains(unitOwner) && players.Count <= 1;
        }

        private void GiveTurnToNextPlayer()
        {
            isComputerPlaying = false;
            currentPlayer.MakeOwnedUnitsUnplayable();
            int nextPlayerIndex = (players.IndexOf(currentPlayer) + 1) % 2;
        
            if (players.ElementAt(nextPlayerIndex) != null)
                currentPlayer = players.ElementAt(nextPlayerIndex);
        }
        
        /// <summary>
        /// Activating only the units that are not marked as defeated in the player's save file
        /// </summary>
        private void ActivatePlayerUnits()
        {
            var characterInfos = Finder.SaveController.GetCurrentSaveSelectedInfos().CharacterInfos;

            foreach (var gameUnit in currentPlayer.OwnedUnits)
            {
                foreach (var saveUnit in characterInfos)
                {
                    if (gameUnit.name == saveUnit.CharacterName && !saveUnit.CharacterStatus)
                    {
                        gameUnit.gameObject.SetActive(false);
                    }
                }
            }
        }
        
        public void IncrementTileUpdate()
        {
            levelTileUpdateKeeper++;
        }
    }
}
