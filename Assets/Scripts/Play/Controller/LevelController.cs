﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Linq;
using Game;
using Harmony;
using UnityEngine;
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
        //[SerializeField] private GameObject dialogueUi = null;
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
        [SerializeField] private Unit unitToProtect = null;
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
            onLevelVictory = new OnLevelVictory();
            Debug.Log("Level name : " + levelName);
        }

        private void Start()
        {
            players.Clear();
            InitializePlayersAndUnits();
            currentPlayer = players[0];
            OnTurnGiven();
            if (dialogueUi != null && dialogueTriggerStartFranklem != null)
            {
                dialogueUi.SetActive(true);
                dialogueTriggerStartFranklem.TriggerDialogue();
            }
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

            
            if (levelCompleted)
            {
                Finder.GameController.LevelsCompleted.Add(levelName);
            }
            cinematicController.LaunchEndCinematic();
            while (cinematicController.IsPlayingACutScene)
            {
                yield return null;
            }
            
            Finder.GameController.LoadLevel(Constants.OVERWORLD_SCENE_NAME);
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
                //TODO: Uncomment below when Turns are available
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
        }

        private void CheckIfLevelFailed()
        {
            //TODO: Uncomment below when Turns are available
            levelFailed =
                defeatIfNotCompleteLevelInCertainAmountOfTurns && (numberOfPlayerTurns >= numberOfTurnsBeforeDefeat) ||
                (defeatIfProtectedIsKilled && unitToProtect.NoHealthLeft) ||
                (defeatIfAllPlayerUnitsDied &&
                 HumanPlayer.Instance.HaveAllUnitsDied()
                ) || (GameObject.Find(PROTAGONIST_NAME) == null || GameObject.Find(PROTAGONIST_NAME).GetComponent<Unit>().NoHealthLeft);
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

        public void CheckForComputerTurnSkip()
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
            UnitOwner player1 = HumanPlayer.Instance;
            UnitOwner player2 = ComputerPlayer.Instance;

            units = FindObjectsOfType<Unit>();

            GiveUnits(units, false, player1, player2);

            player1.UpdateNumberOfStartingOwnedUnits();
            player1.OnNewLevel();
            player2.OnNewLevel();
            
            players.Add(player1);
            players.Add(player2);
        }

        private void GiveUnits(Unit[] units, bool isEnemy, UnitOwner player, UnitOwner aiPlayer)
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
        }

        public void CheckForCurrentPlayerEndOfTurn()
        {
            if (currentPlayer.HasNoMorePlayableUnits)
            {
                GiveTurnToNextPlayer();
                OnTurnGiven();
            }
        }

        public void CheckForCurrentPlayerWin()
        {
            if (HasWon(currentPlayer))
            {
                currentPlayer.Win();
            }
        }

        public void CheckForCurrentPlayerLoss()
        {
            if (currentPlayer.HaveAllUnitsDied())
            {
                currentPlayer.Lose();
                UnitOwner playerWhoLost = currentPlayer;
                GiveTurnToNextPlayer();
            }
        }

        public bool HasWon(UnitOwner unitOwner)
        {
            return players.Contains(unitOwner) && players.Count <= 1;
        }

        public void GiveTurnToNextPlayer()
        {
            isComputerPlaying = false;
            currentPlayer.MakeOwnedUnitsUnplayable();
            int nextPlayerIndex = (players.IndexOf(currentPlayer) + 1) % 2;
        
            if (players.ElementAt(nextPlayerIndex) != null)
                currentPlayer = players.ElementAt(nextPlayerIndex);
        }
        
        public void IncrementTileUpdate()
        {
            levelTileUpdateKeeper++;
        }
    }
}
